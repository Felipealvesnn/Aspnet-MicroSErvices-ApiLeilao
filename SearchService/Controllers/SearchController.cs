
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        public async Task<ActionResult<List<Item>>> SeachItems(
         [FromQuery] SearchParams searchParams)

        {
            var query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
            }


            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make))
                    .Sort(x => x.Ascending(a => a.Model)),
                "new" => query.Sort(x => x.Descending(a => a.CreateAT)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x =>
                   x.AuctionEnd > DateTime.UtcNow &&
                   x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                   ),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }
            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Winner == searchParams.Winner);
            }

            query.PageNumber(searchParams.PageNumber).PageSize(searchParams.PageSize);
            var result = await query.ExecuteAsync();

            return Ok(new
            {
                result = result.Results,
                currentPage = result.PageCount,
                total = result.TotalCount,
            });
        }
    }
}
