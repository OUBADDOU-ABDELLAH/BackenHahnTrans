using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HahnTransportAutomate.Services
{
    public class CargoTransporterService : ICargoTransporterService
    {

        private readonly ILogger<CargoTransporterService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ItransporterRepository transporterRepository;
        public CargoTransporterService(ILogger<CargoTransporterService> logger, IHttpClientFactory httpClient,  ItransporterRepository transporterRepository)
        {
            this.logger = logger;
            httpClientFactory = httpClient;
            this.transporterRepository = transporterRepository;
        }

        public async Task<int> Buy(int positionNodeId,string username, string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync($"CargoTransporter/Buy?positionNodeId={positionNodeId}", null);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to Buy TransporterRepository from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return -3;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                int transporterId = JsonConvert.DeserializeObject<int>(responseContent);
                TransporterDto transInfo = new TransporterDto { TransId=transporterId,UserName=username};
                await transporterRepository.AddTransporterAsync(transInfo);
                return transporterId;
            }
        }


        public async Task<CargoTransporterDto?> Get(int transporterId, string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"CargoTransporter/Get?transporterId={transporterId}");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to get CargoTransporterService from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return null;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var CargoTransporterResult = JsonConvert.DeserializeObject<CargoTransporterDto>(responseContent);
                return CargoTransporterResult;
            }
        }

        public async Task<List<int>> GetListTransIdByUserName(string username)
        {
            return await transporterRepository.GetTransporterByUserNameAsync(username);
        }

        public async Task<bool> Move(int transporterId, int targetNodeId, string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsync($"CargoTransporter/Move?transporterId={transporterId}&targetNodeId={targetNodeId}", null);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to put Move TransporterRepository from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
