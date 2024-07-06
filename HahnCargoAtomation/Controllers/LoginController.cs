using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Models;
using HahnTransportAutomate.Services;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Claims;

namespace HahnTransportAutomate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ICargoTransporterService transService;
        private readonly IStrategyService strategyService;



        public LoginController(IUserService userService, ICargoTransporterService transService, IStrategyService strategyService)
        {
            this.userService = userService;
            this.transService = transService;
            this.strategyService = strategyService;
        }

        [HttpGet("Coins")]
        public async Task<int> CoinAmount(string username)
        {
            string token = await HahnTransporterHelper.getTokenByUserName(username) ?? await userService.GetTokenByUserName(username);
            int coins =await userService.GetCoins(token);
            return coins;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<List<int>> Login([FromBody] UserAuthenticationDto model)
        {
            List<int> transportersId = null;
            var transId=await strategyService.BuyingTransDecisionLogin(model);
            if (transId == -3)
            {
                return null;
            }
            else
            {
                transportersId = await transService.GetListTransIdByUserName(model.UserName);
                if (transportersId.Count() > 0)
                {
                    return transportersId;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
