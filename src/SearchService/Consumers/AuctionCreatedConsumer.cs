using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine($"conbsumer foi criaro {context.Message}");

            var item = _mapper.Map<Item>(context.Message);

            if (item.Model == "Foo")
            {
                throw new ArgumentNullException(nameof(item));
            }
            await item.SaveAsync();

        }
    }
}
