using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AuctionDbContext: DbContext
    {
        public AuctionDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Entities.Auction> Auctions { get; set; }

        

    }
}
