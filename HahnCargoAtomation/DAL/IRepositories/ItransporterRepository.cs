using HahnTransportAutomate.DTOs;

namespace HahnTransportAutomate.DAL.IRepositories
{
    public interface ItransporterRepository
    {
        Task<int> AddTransporterAsync(TransporterDto entity);
        Task<List<int>> GetTransporterByUserNameAsync(string username);
    }
}
