using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Models;

namespace HahnTransportAutomate.Services.Interfaces
{
    public interface IStrategyService
    {
        Task<ResultBuyCarDto> TransPositionNodeId(string token);
        Task<int> BuyingTransDecisionLogin(UserAuthenticationDto user);
    }
}
