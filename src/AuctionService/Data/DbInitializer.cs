using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class DbInitializer
    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            SeedData(scope.ServiceProvider.GetService<AuctionDbContext>());
        }

        private static void SeedData(AuctionDbContext context)
        {
            context.Database.Migrate();
            if (context.Auctions.Any())
            {
                Console.WriteLine("Ja foi tudo alimentado.");
                return;
            }
        }
    }
}
