using HahnTransportAutomate.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HahnTransportAutomate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly IGridService gridService;
        private readonly IUserService userService;
        public GridController(IGridService gridService, IUserService userService)
        {
            this.gridService = gridService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<string> Get(string username)
        {
            string token = await userService.GetTokenByUserName(username);
            var getGrid = await gridService.GetGridAsJson(token);
            return getGrid;
        }
    }
}
