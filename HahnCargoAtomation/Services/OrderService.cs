using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using HahnTransportAutomate.Models;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Services.Interfaces;
using HahnTransportAutomate.DAL.IRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace HahnTransportAutomate.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICargoTransporterService cargoTransporterService;
        private readonly IGridService gridService;
        private readonly IOrderManager orderManager;
        private readonly IUserService userService;
        private readonly IStrategyService strategyService;
        private readonly ITokenRepository tokenRepository;

        public OrderService(ICargoTransporterService cargoTransporterService, IGridService gridService, IOrderManager orderManager, IUserService userService, IStrategyService strategyService, ITokenRepository tokenRepository)
        {
            this.cargoTransporterService = cargoTransporterService;
            this.gridService = gridService;
            this.orderManager = orderManager;
            this.userService = userService;
            this.strategyService = strategyService;
            this.tokenRepository = tokenRepository;
        }
        public async Task<bool> AcceptedOrder(int transporterId, string username)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            bool decision = false;
            string token = await HahnTransporterHelper.getTokenByUserName(username) ?? await tokenRepository.GetTokenByUserNameAsync(username);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
                var order = System.Text.Json.JsonSerializer.Deserialize<OrderDto>(message, options);
                await DecideToAcceptAnOrder(order, transporterId,username, token);
            };
            channel.BasicConsume(queue: "HahnCargoSim_NewOrders", autoAck: true, consumer: consumer);
            return decision;
        }

        private async Task<bool> DecideToAcceptAnOrder(OrderDto order, int transporterId, string username,string token)
        {
            bool result = false;
            CargoTransporterDto cargoTransporterDto = await cargoTransporterService.Get(transporterId, token);
            if (cargoTransporterDto == null)
            {
                return result;
            }
            int startNodeId = cargoTransporterDto.PositionNodeId;

            string gridContent = await HahnTransporterHelper.getGridFromCach(username);
            Grid gridResult;
            if (gridContent == null)
            {
                return result;
            }
            else
            {
                gridResult = JsonConvert.DeserializeObject<Grid>(gridContent);
            }
            List<Connection> connectionList = gridResult?.Connections;
            bool isConAvailable = gridService.ConnectionAvailable(connectionList, startNodeId, order.TargetNodeId);
            int userCoins = 0;
            if (isConAvailable is true)
            {
                userCoins = await userService.GetCoins(token);
                if (userCoins > 0)
                {
                    result = await orderManager.Accept(order.Id, token);
                }
            }
            else
            {
                return result;
            }
            if (result is true)
            {
                result = await cargoTransporterService.Move(transporterId, order.TargetNodeId, token);
            }
            if(userCoins >= 2000)
            {
                ResultBuyCarDto order_positionSatrtNodeId = await strategyService.TransPositionNodeId(token);
                int transId = await cargoTransporterService.Buy(order_positionSatrtNodeId.PositionNodeId, username,token);
                bool acceptorder = await orderManager.Accept(order_positionSatrtNodeId.OrderId, token);
                if (acceptorder is true)
                {
                    result = await cargoTransporterService.Move(transId, order.TargetNodeId, token);

                }

            }
            if (userCoins == 0)
            {
                await orderManager.StopSim(token);
            }
            return result;

        }

    }
}
