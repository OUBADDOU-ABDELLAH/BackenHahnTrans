using HahnTransportAutomate.DTOs;
using StackExchange.Redis;

namespace HahnTransportAutomate.Services.Interfaces
{
    public interface ICargoTransporterService
    {
        Task<CargoTransporterDto?> Get(int transporterId, string token);
        Task<int> Buy(int positionNodeId, string username, string token);
        Task<bool> Move(int transporterId, int targetNodeId, string username);
        Task<List<int>> GetListTransIdByUserName(string username);
    }
}
