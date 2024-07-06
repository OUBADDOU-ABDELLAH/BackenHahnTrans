using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Models;

namespace HahnTransportAutomate.Services.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponseModel> LoginToken(UserAuthenticationDto userInfo);
        Task<int> GetCoins(string token);

        Task<string> GetTokenByUserName(string username); 

    }
}
