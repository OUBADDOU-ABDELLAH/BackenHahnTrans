using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Models;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Collections.Generic;

namespace HahnTransportAutomate.Services
{
    public class StrategyService : IStrategyService
    {
        private readonly IOrderManager orderManager;
        private readonly IGridService gridService;
        private readonly ITokenRepository tokenRepository;
        private readonly ICargoTransporterService transService;
        private readonly IUserService userService;
        public StrategyService(IOrderManager orderManager, IGridService gridService, ITokenRepository tokenRepository, ICargoTransporterService transService, IUserService userService)
        {
            this.orderManager = orderManager;
            this.gridService = gridService;
            this.tokenRepository = tokenRepository;
            this.transService = transService;
            this.userService = userService;
        }


        private ResultBuyCarDto GetMaxCostOriginNodeId(List<OrderDto> orders, Grid grid)
        {
            var originNodeWithMaxCost = (from order in orders
                                         from connection in grid?.Connections
                                         where connection.FirstNodeId == order.OriginNodeId
                                         from edge in grid.Edges
                                         where edge.Id == connection.EdgeId
                                         orderby edge.Cost descending
                                         select new {order.Id, order.OriginNodeId, edge.Cost })
                                         .FirstOrDefault();
            return new ResultBuyCarDto {OrderId = originNodeWithMaxCost.Id ,PositionNodeId= originNodeWithMaxCost.OriginNodeId }; ;
        }

        public async Task<ResultBuyCarDto> TransPositionNodeId(string token)
        {
            try
            {
                var availableorders = await orderManager.GetAllAvailableOrders(token);
                if (availableorders !=null & availableorders.Count()!=0)
                {
                    var grid = await gridService.GetGridAsJson(token);
                    if(!string.IsNullOrEmpty(grid))
                    {
                        var gridResult = JsonConvert.DeserializeObject<Grid>(grid);
                        return GetMaxCostOriginNodeId(availableorders, gridResult);
                    }
                    else
                    {
                        return null;//new ResultBuyCarDto { PositionNodeId = 1 };
                    }

                }
                else
                {
                    return null;//new ResultBuyCarDto { PositionNodeId = 1};
                }
   
            }
            catch (Exception ex)
            {
                //je vais ajouter les logs
                return null;
            }
        }

        public async Task<int> BuyingTransDecisionLogin(UserAuthenticationDto user)
        {

            string userInfo = await tokenRepository.GetTokenByUserNameAndPasswordAsync(user);
            int buyTrans;
            ResultBuyCarDto resultBuyTrans = null;
            TokenResponseModel createUserResult = null ;
            if (userInfo=="")
            {
                 createUserResult = await userService.LoginToken(user);
            }
            if (userInfo == null)
            {
                return -3;
            }
            else
            {
                HahnTransporterHelper.setTokenByUserName(user.UserName, userInfo);
                //get usercoins
                //token = createUserResult.Token;
                int coins = await userService.GetCoins(userInfo);
                if (coins >= 2000)
                {
                    resultBuyTrans = await TransPositionNodeId(userInfo);
                    if (resultBuyTrans != null)
                    {
                        buyTrans = await transService.Buy(resultBuyTrans.PositionNodeId, user.UserName, userInfo);
                        return buyTrans;
                    }
                    else
                    {
                        //There is no available orders which means it's not beneficial to buy a transporter
                        return -2;
                    }
                }
                
            }
            if (createUserResult != null)
            {
                HahnTransporterHelper.setTokenByUserName(user.UserName, createUserResult.Token);
                resultBuyTrans = await TransPositionNodeId(createUserResult.Token);
                if(resultBuyTrans != null)
                {
                    buyTrans = await transService.Buy(resultBuyTrans.PositionNodeId, user.UserName, createUserResult.Token);
                    return buyTrans;
                }
                else
                {
                    int defaultPositionStartNodeId = 1;
                    buyTrans = await transService.Buy(defaultPositionStartNodeId, user.UserName, createUserResult.Token);
                    return buyTrans;
                }

            }
            if (userInfo != "")
            {

                return 0;
            }
            else
            {
                return -3;
            }
        }
    }
}
