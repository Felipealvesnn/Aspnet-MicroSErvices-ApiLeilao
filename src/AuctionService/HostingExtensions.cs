namespace AuctionService
{
    public static class HostingExtensions
    {
        public static IServiceCollection ConfigureServicesJwt(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["IdentityServiceUrl"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.NameClaimType = "username";

                    options.Audience = "AuctionService";
                });


            return Services;
        }
    }
}
