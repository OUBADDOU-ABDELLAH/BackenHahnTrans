using HahnTransportAutomate.DTOs;
using System.Net.Http.Headers;
using System.Net.Http;

namespace HahnTransportAutomate.Services.Interfaces
{
    public interface IOrderManager
    {
        Task<bool> Accept(int orderid, string token);
        //Task<List<Order>> GetAllAvailable();
        Task<List<OrderDto>> GetAllAcceptedOrders(string token);
        Task<List<OrderDto>> GetAllAvailableOrders(string token);
        Task<int> StopSim(string token);
    }
}
