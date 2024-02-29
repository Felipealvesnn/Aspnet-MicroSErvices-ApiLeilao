
using Microsoft.AspNetCore.Mvc;
using SearchService.models;

namespace SearchService.Controllers
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
