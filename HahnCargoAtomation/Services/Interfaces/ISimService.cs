namespace HahnTransportAutomate.Services.Interfaces
{
    public interface ISimService
    {
        Task<int> Start(int transporterId, string username);
        Task RunTick(int tick);

    }
}
