using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController, Route("api/auctions")]
    public class HomeController : ControllerBase
    {
        private readonly AuctionDbContext _auctionDb;
        private readonly IMapper _mapper;

        public HomeController(AuctionDbContext auctionDb, IMapper mapper)
        {
            _auctionDb = auctionDb;
            _mapper = mapper;
        }

        public async Task<ActionResult<List<AuctionDto>>> Index()
        {
            var auctions = await _auctionDb.Auctions
                .Include(a => a.Item).OrderBy(x=>x.Item.Model)
                .ToListAsync();
         
            var auctionsDto = _mapper.Map<List<AuctionDto>>(auctions);
            return Ok(auctionsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            var auction = await _auctionDb.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return Ok(auctionDto);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            _auctionDb.Auctions.Add(auction);
            await _auctionDb.SaveChangesAsync();
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auctionDto);
        }


    }
}
