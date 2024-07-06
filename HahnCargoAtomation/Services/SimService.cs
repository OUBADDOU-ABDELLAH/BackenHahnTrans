using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Services.Interfaces;
using HahnTransportAutomate.DAL.Repositories;
using HahnTransportAutomate.DAL.IRepositories;

namespace HahnTransportAutomate.Services
{
    public class SimService : ISimService
    {
        private readonly IOrderService orderService;
        private readonly IGridService gridService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IUserService userService;

        bool isrunning = false;
        public int transporterId { get; set; }
        public string username { get; set; }
        public SimService(IOrderService orderService, IGridService gridService, IHttpClientFactory httpClientFactory, IUserService userService)
        {
            isrunning = false;
            this.orderService = orderService;
            this.gridService = gridService;
            this.httpClientFactory = httpClientFactory;
            this.userService = userService;
        }

        private void SetOrderParameters(int transporterId, string username)
        {
            this.transporterId = transporterId;
            this.username = username;
        }
        public async Task<int> Start(int transporterId, string username)
        {
            string tokenResult = await HahnTransporterHelper.getTokenByUserName(username) ?? await userService.GetTokenByUserName(username);
            int resStartSim=await startSim(tokenResult);
            string gridContent = await gridService.GetGridAsJson(tokenResult);
            if (gridContent != null)
            {
                HahnTransporterHelper.setGridInCachMemory(username, gridContent);
            }
            SetOrderParameters(transporterId, username);
            isrunning = true;
            return resStartSim;
        }

        private async Task<int> startSim(string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("Sim/Start", null);
            if (!response.IsSuccessStatusCode)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public async Task RunTick(int tick)
        {
            if (isrunning is true)
            {
                await orderService.AcceptedOrder(transporterId, username);
            }
        }

        public async Task<int> StopSim(string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("Sim/Stop", null);
            if (!response.IsSuccessStatusCode)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
