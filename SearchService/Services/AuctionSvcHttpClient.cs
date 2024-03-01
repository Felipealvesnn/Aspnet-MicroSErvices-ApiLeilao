using MongoDB.Entities;
using SearchService.models;

namespace SearchService.Services
{
    public class AuctionSvcHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionSvcHttpClient(HttpClient httpClient,
            IConfiguration config )
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Item>> GetItensForSearchdb()
        {
            var lastUtade = await DB.Find<Item, string>()
                .Sort(x=>x.Descending(x=>x.UpdatedAt))
                .Project(x=> x.UpdatedAt.ToString())
                .ExecuteFirstAsync();
            var url = _config["AuctionServiceUrl"];
            var stringnec = _config["urlladecara"];

            var model = await _httpClient.GetFromJsonAsync<List<Item>>(
                $"{url}/api/auctions?date={lastUtade}");

            return model;
        }

    }
}
