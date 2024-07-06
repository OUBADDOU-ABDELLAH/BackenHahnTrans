using HahnTransportAutomate.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HahnTransportAutomate.DAL
{
    public class HahnTransDbContext : DbContext
    {
        public HahnTransDbContext(DbContextOptions<HahnTransDbContext> options): base(options)
        {
        }

        public DbSet<TokenDto> Token { get; set; }
        public DbSet<TransporterDto> Transporter { get; set; }

    }
}
