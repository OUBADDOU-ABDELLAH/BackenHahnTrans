using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel;
using System.Security.Claims;

namespace HahnTransportAutomate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager orderManager;
        private readonly IOrderService orderService;
        private readonly IUserService userService;
        public OrderController(IOrderManager orderManager, IOrderService orderService, IUserService userService)
        {
            this.orderManager = orderManager;
            this.orderService = orderService;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Accept(int orderId, string username)
        {
            string token = await userService.GetTokenByUserName(username);
            var AcceptOrder = await orderManager.Accept(orderId, token);
            return AcceptOrder == true ? Ok("Oreder Accepted") : BadRequest("Order Not Accepted");
        }

        [HttpPut]
        public async Task<IActionResult> OrderAccepted(int transporterId, string username)
        {
            var AcceptOrder = await orderService.AcceptedOrder(transporterId, username);
            return AcceptOrder == true ? Ok("Oreder Accepted") : BadRequest("Order Not Accepted");
        }

        [HttpGet]
        public async Task<ResponseDto> getAllAcceptedOrders(string username)
        {
            string token = await HahnTransporterHelper.getTokenByUserName(username) ?? await userService.GetTokenByUserName(username);  
            List<OrderDto> AcceptOrders = await orderManager.GetAllAcceptedOrders(token);
            int coins = await userService.GetCoins(token);
            ResponseDto response = new ResponseDto();
            response.orders = AcceptOrders;
            response.coins = coins;
            return response;
        }
    }
}
