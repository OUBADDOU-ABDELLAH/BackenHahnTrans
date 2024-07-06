using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HahnTransportAutomate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoTransporterController : ControllerBase
    {
        private readonly ICargoTransporterService transporterService;
        private readonly IUserService userService;
        public CargoTransporterController(ICargoTransporterService transporterService, IUserService userService)
        {
            this.transporterService = transporterService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<CargoTransporterDto?> Get(int transporterId, string username)
        {
            string token = await userService.GetTokenByUserName(username);
            var transporter = await transporterService.Get(transporterId, token);
            return transporter;
        }
        [HttpPost]
        public async Task<int> Buy(int positionNodeId, string username)
        {
            string token = await userService.GetTokenByUserName(username);
            int transporterId = await transporterService.Buy(positionNodeId, username,token);
            return transporterId;
        }

    }
}
