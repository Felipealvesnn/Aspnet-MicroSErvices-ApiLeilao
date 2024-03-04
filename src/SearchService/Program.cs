using MassTransit;

using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.models;
using SearchService.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
});
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("Procurar", false)); 
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("procurar", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();



app.Lifetime.ApplicationStarted.Register(async () =>
{
    await Policy.Handle<TimeoutException>()
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(10))
        .ExecuteAndCaptureAsync(async () =>
        await DbInitializer.InitDb(app));

});

app.MapControllers();







app.Run();

//Isso aqui é pra ele tentar a requisiçao pelo menos 6 vezes antes de desistir
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
{

    var result = HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)));

    return result;
}
