
namespace HahnTransportAutomate.Services.Interfaces
{
    public interface IOrderService
    {
        Task<bool> AcceptedOrder(int transporterId, string username);
    }
}
