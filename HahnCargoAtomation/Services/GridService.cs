using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using HahnTransportAutomate.Models;
using HahnTransportAutomate.Helper;
using HahnTransportAutomate.Services.Interfaces;
using HahnTransportAutomate.DAL.Repositories;
using HahnTransportAutomate.DAL.IRepositories;

namespace HahnTransportAutomate.Services
{
    public class GridService : IGridService
    {
        private readonly ILogger<GridService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        public GridService(ILogger<GridService> logger, IHttpClientFactory httpClient)
        {
            this.logger = logger;
            httpClientFactory = httpClient;
        }

        public bool ConnectionAvailable(List<Connection> connections, int sourceNodeId, int targetNodeId)
        {
            if (connections == null)
            {
                return false;
            }

            var res = (from connection in connections
                       where connection.FirstNodeId == sourceNodeId && connection.SecondNodeId == targetNodeId
                       select connection.Id).FirstOrDefault();

            return res != 0;
        }


        public int GetConnectionCost(Grid grid, int connectionId)
        {
            var con = GetConnection(grid, connectionId);
            Edge? edge = null;
            if (con != null)
            {
                edge = GetEdge(grid, con.EdgeId);
            }
            return edge?.Cost ?? -1;
        }
        private Connection? GetConnection(Grid grid, int Id)
        {
            return grid.Connections.Find(c => c != null && c.Id == Id);
        }
        private Edge? GetEdge(Grid grid, int Id)
        {
            return grid.Edges.Find(e => e != null && e.Id == Id);
        }


        public async Task<string> GetGridAsJson(string token)
        {
            var client = httpClientFactory.CreateClient("HahnSim");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("Grid/Get");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Unable to get CargoTransporterService from Hahn simulation API \n Error : {response.ReasonPhrase}");
                return null;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                //var gridResult = JsonConvert.DeserializeObject<Grid>(responseContent);
                return responseContent;
            }
        }
    }
}
