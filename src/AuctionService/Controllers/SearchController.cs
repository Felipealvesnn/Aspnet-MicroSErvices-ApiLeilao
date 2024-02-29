using AuctionService.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        public async Task<ActionResult<Item>> SeachItems(string searchterm)
        {
            return Ok();
        }
    }
}
