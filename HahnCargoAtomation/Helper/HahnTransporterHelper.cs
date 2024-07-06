using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HahnTransportAutomate.Helper
{
    public class HahnTransporterHelper
    {
        public static bool isBuyingCargoFirstTime = false;
        public static async Task setTransporterIdInCachMemory(string key, int transporterId)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db = redis.GetDatabase();

                await db.StringSetAsync("transporterId" + key, transporterId.ToString(), TimeSpan.FromDays(1));

            }
        }


        public async static Task<int> getTransporterIdFromCach(string key)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db = redis.GetDatabase();

                var grid = await db.StringGetAsync("transporterId" + key);
                return (int)grid;

            }

        }

        public static async Task setGridInCachMemory(string key, string Content)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db = redis.GetDatabase();

                await db.StringSetAsync("gridResult" + key, Content.ToString(), TimeSpan.FromDays(1));

            }
        }
        public async static Task<string> getGridFromCach(string key)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db = redis.GetDatabase();

                var grid = await db.StringGetAsync("gridResult" + key);
                return grid;

            }

        }
        public static async Task setTokenByUserName(string userName, string token)
        {

            using (var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db = redis.GetDatabase();
                if (!string.IsNullOrEmpty(userName))
                {
                    await db.StringSetAsync(userName, token, TimeSpan.FromDays(1));
                }
            }
        }
        public async static Task<string> getTokenByUserName(string userName)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db = redis.GetDatabase();
                if (!string.IsNullOrEmpty(userName))
                {
                    var token = await db.StringGetAsync(userName);
                    return token.ToString();
                }
                else
                {
                    return null;
                }
            }

        }



    }
}
