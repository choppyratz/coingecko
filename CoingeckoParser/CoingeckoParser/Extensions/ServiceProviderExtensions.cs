using CoingeckoParser.Services;
using CoingeckoParser.Services.Contracts;

namespace CoingeckoParser.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddBussinessServices(this IServiceCollection services)
        {
            services.AddTransient<ICoingeckoApiService, CoingeckoApiService>();
            services.AddHostedService<PriceWatcherService>();
        }
    }
}
