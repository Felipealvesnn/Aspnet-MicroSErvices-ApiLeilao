﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace AuctionService.Controllers
{
    [ApiController, Route("api/auctions")]
    //[Authorize]
    public class HomeController : ControllerBase
    {
        private readonly AuctionDbContext _auctionDb;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public HomeController(AuctionDbContext auctionDb, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _auctionDb = auctionDb;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ActionResult<List<AuctionDto>>> Index(string? date)
        {
            var query = _auctionDb.Auctions
                .Include(a => a.Item).OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {

                query = query.Where(x => x.UpdateAt.CompareTo(DateTime.Parse(date)
                    .ToUniversalTime()) > 0);

            }
            var model = await query.ProjectTo<AuctionDto>(
                _mapper.ConfigurationProvider).ToListAsync();




            return Ok(model);
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
        // [Route("CreateAuction")]

        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            //  createAuctionDto.AuctionEnd = createAuctionDto.AuctionEnd.o();
            var auction = _mapper.Map<Auction>(createAuctionDto);
            auction.Seller = User.Identity.Name;
            _auctionDb.Auctions.Add(auction);

            var auctionDto = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(auctionDto));


            var result = await _auctionDb.SaveChangesAsync() > 0;

            if (!result) return BadRequest();





            return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auctionDto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _auctionDb.Auctions.FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null) return NotFound();

            if (auction.Seller != User.Identity.Name) return Forbid();

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            var result = await _auctionDb.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Error na hora de salvar");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _auctionDb.Auctions.FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null) return NotFound();

            if(auction.Seller != User.Identity.Name) return Forbid();

            _auctionDb.Remove(auction);

            var result = await _auctionDb.SaveChangesAsync() > 0;
            await _publishEndpoint.Publish(_mapper.Map<AuctionDeleted>(new
            {
                Id = auction.Id.ToString()
            }));

            if (result) return Ok();

            return BadRequest("Error na hora de deletar");
        }



    }
}
