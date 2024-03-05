var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));



builder.Services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", options =>
               {
                   options.Authority = builder.Configuration["IdentityServiceUrl"];
                   options.RequireHttpsMetadata = false;
                   options.TokenValidationParameters.ValidateAudience = false;
                   options.TokenValidationParameters.NameClaimType = "username";

                   options.Audience = "AuctionService";
               });



var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
