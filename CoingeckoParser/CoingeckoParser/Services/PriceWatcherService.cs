using CoingeckoParser.Services.Contracts;

namespace CoingeckoParser.Services
{
    public class PriceWatcherService : IHostedService
    {
        private readonly ICoingeckoApiService _coingeckoApiService;
        private Timer _timer;

        public PriceWatcherService(ICoingeckoApiService coingeckoApiService)
        {
            _coingeckoApiService = coingeckoApiService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(MonitorPrices, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private async void MonitorPrices(object state)
        {
            await _coingeckoApiService.GetCoinPrices();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
