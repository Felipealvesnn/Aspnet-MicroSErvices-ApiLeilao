using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.models;
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
            if (count == 0)
            {
                Console.WriteLine("Tem nada no Mongo");
                var itemData = await File.ReadAllTextAsync("Data/auctions.json");
                var items = JsonSerializer.Deserialize<List<Item>>(itemData);

                await DB.SaveAsync(items);
            }


        }
    }
}
