using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HahnTransportAutomate.DAL.Repositories
{
    public class TransporterRepository : ItransporterRepository
    {
        private readonly HahnTransDbContext context;
        public TransporterRepository(HahnTransDbContext context)
        {
            this.context = context;
        }

        public async Task<int> AddTransporterAsync(TransporterDto entity)
        {
            try
            {
                await context.Transporter.AddAsync(entity);
                await context.SaveChangesAsync();
                return 1;
            }
            catch (DbUpdateException ex)
            {
                return -1;
            }
            catch (SqlException ex)
            {
                return -2;
            }

            catch (Exception ex)
            {
                return -3;
            }
        }

        public async Task<List<int>> GetTransporterByUserNameAsync(string username)
        {
            var transporters = await context.Transporter.Where(x => x.UserName == username).Select(x => x.TransId).ToListAsync(); 
            if(transporters!=null)
            {
                return transporters;
            }
            else
            {
                return null;
            }
        }
    }
}
