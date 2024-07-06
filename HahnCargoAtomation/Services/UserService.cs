using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Models;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;

namespace HahnTransportAutomate.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ITokenRepository tokenRepository;
        public UserService(IConfiguration configuration, ILogger<UserService> logger, IHttpClientFactory httpClient, ITokenRepository tokenRepository = null)
        {
            this.configuration = configuration;
            this.logger = logger;
            httpClientFactory = httpClient;
            this.tokenRepository = tokenRepository;
        }

        public async Task<int> GetCoins(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                logger.LogError($"Unable to get TokenDto from Hahn simulation API");
                return -1;
            }
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("User/CoinAmount");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to get coins from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return -2;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var coins = JsonConvert.DeserializeObject<int>(responseContent);
                return coins;
            }
        }

        public async Task<string> GetTokenByUserName(string username)
        {
            return await tokenRepository.GetTokenByUserNameAsync(username);
        }

        public async Task<TokenResponseModel> LoginToken(UserAuthenticationDto userInfo)
        {

            var postMessage = new Dictionary<string, string>
            {
                { "username", userInfo.UserName },
                { "password", userInfo.Password }
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(postMessage), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue(configuration["contenttype"]);
            var client = httpClientFactory.CreateClient("HahnSim");
            logger.LogInformation("\n------------------------------ Begin get token method -------------------------------\n");
            var response = await client.PostAsync("User/Login", content);



            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to get token from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return null;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var resultToken = JsonConvert.DeserializeObject<TokenResponseModel>(responseContent);
                logger.LogInformation("\n------------------------------ End get token method -----------------------------------\n");

                TokenDto addToken = new TokenDto { Username = userInfo.UserName, TokenValue = resultToken.Token }; 
                await tokenRepository.AddTokenAsync(addToken);
                return new TokenResponseModel
                {
                    Token = resultToken.Token,
                    UserName = resultToken.UserName
                };
            }

        }
    }
}
