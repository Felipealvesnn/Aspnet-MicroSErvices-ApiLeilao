using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.models;
using SearchService.Services;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings.
                FromConnectionString(app.Configuration
                .GetConnectionString("MongoDbConnection")
          ));

            await DB.Index<Item>()
                .Key(a => a.Make, KeyType.Text)
                .Key(a => a.Model, KeyType.Text)
                .Key(a => a.Year, KeyType.Text)
                .Key(a => a.Color, KeyType.Text)
                .Key(a => a.Mileage, KeyType.Text)
                .Key(a => a.Status, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();
           
            using var scope = app.Services.CreateScope();
            var htppclient  = scope.ServiceProvider.
                GetRequiredService<AuctionSvcHttpClient>();

            var item = await htppclient.GetItensForSearchdb();

            if(item.Count >0) await DB.SaveAsync(item);

        }
    }
}
