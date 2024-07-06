using HahnTransportAutomate.DTOs;

namespace HahnTransportAutomate.DAL.IRepositories
{
    public interface ITokenRepository 
    {
        Task<int> AddTokenAsync(TokenDto entity);
        Task<string> GetTokenByUserNameAsync(string username);    
        Task<string> GetTokenByUserNameAndPasswordAsync(UserAuthenticationDto user);    
    }
}
