using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DAL.Repositories;
using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HahnTransportAutomate.Services
{
    public class OrderManager : IOrderManager
    {
        private readonly ILogger<OrderManager> logger;
        private readonly IHttpClientFactory httpClientFactory;
        public OrderManager(ILogger<OrderManager> logger, IHttpClientFactory httpClient)
        {
            this.logger = logger;
            httpClientFactory = httpClient;
        }
        public async Task<bool> Accept(int orderid, string token)
        {

            if (string.IsNullOrEmpty(token))
            {
                logger.LogError($"Unable to get TokenDto from Hahn simulation API");
                return false;
            }
            var postMessage = new Dictionary<string, string> { };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(postMessage), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            logger.LogInformation("\n------------------------------ Begin  -------------------------------\n");
            var response = await client.PostAsync($"Order/Accept?orderId={orderid}", content);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to accept order from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return false;
            }
            else
            {
                //bool responseContent = Boolean.Parse(await response.Content.ReadAsStringAsync());
                logger.LogInformation("\n------------------------------ End get token method -----------------------------------\n");
                return true;
            }
        }
        public async Task<List<OrderDto>> GetAllAvailableOrders(string token)
        {
  
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("Order/GetAllAvailable");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to get available orders from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return null;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var AvailableOrders = JsonConvert.DeserializeObject<List<OrderDto>>(responseContent);
                return AvailableOrders;
            }
        }
        public async Task<List<OrderDto>> GetAllAcceptedOrders(string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("Order/GetAllAccepted");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to get accepted orders from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return null;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var AcceptedOrders = JsonConvert.DeserializeObject<List<OrderDto>>(responseContent);
                return AcceptedOrders;
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
