using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HahnTransportAutomate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimController : ControllerBase
    {
        private readonly ISimService simService;
        private readonly IOrderManager orderManager;
        private readonly IUserService userService;

        public SimController(ISimService simService, IOrderManager orderManager, IUserService userService)
        {
            this.simService = simService;
            this.orderManager = orderManager;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Start(int transporterId, string username)
        {
            var res = await simService.Start(transporterId, username) ;
            if (res == 1)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("stop")]
        public async void Stop( string username)
        {
            string token = await HahnTransporterHelper.getTokenByUserName(username) ?? await userService.GetTokenByUserName(username);
            await orderManager.StopSim(token);
        }
    }
}
