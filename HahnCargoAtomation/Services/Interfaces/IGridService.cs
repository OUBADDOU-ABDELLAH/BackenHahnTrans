using HahnTransportAutomate.Models;

namespace HahnTransportAutomate.Services.Interfaces
{
    public interface IGridService
    {
        Task<string> GetGridAsJson(string token);
        public bool ConnectionAvailable(List<Connection> connections, int sourceNodeId, int targetNodeId);
        public int GetConnectionCost(Grid grid, int connectionId);

    }
}
