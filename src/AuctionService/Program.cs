using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.RequestHelpers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;


        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AuctionDbContext>(
    opt =>
    {
        opt.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"));

    }
);
builder.Services.AddMassTransit(x =>
{

    //x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("Leilao", false));
    //x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
      //servico de outbox do banco, ficar msg q tem algo la  
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o => { 
    
        o.QueryTimeout = TimeSpan.FromSeconds(30);
        o.UsePostgres();
        o.UseBusOutbox();
    
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();



app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.Initialize(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}


app.Run();
