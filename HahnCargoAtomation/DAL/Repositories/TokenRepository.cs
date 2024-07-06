using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DTOs;
using HahnTransportAutomate.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HahnTransportAutomate.DAL.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly HahnTransDbContext context;
        public TokenRepository(HahnTransDbContext context) 
        {
            this.context = context;
        }

        public async Task<int> AddTokenAsync(TokenDto entity)
        {
            try
            {
                await context.Token.AddAsync(entity);
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
        public async Task<string> GetTokenByUserNameAsync(string username)
        {
            try
            {
                var token = await context.Token.Where(x => x.Username == username).FirstOrDefaultAsync();
                if (token != null)
                {
                    return token.TokenValue;
                }
                else
                {
                    return await HahnTransporterHelper.getTokenByUserName(username);
                }
            }
            catch (Exception ex)
            {
                return "";
            }

            
        }

        public async Task<string> GetTokenByUserNameAndPasswordAsync(UserAuthenticationDto user)
        {
            try
            {
                if (user.Password == "Hahn")//Juste To speed up the developement i know it should be done in the database
                {
                    var token = await context.Token.Where(x => x.Username == user.UserName).FirstOrDefaultAsync();
                    if (token != null)
                    {
                        return token.TokenValue;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
