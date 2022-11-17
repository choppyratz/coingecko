using CoingeckoParser.Models;

namespace CoingeckoParser.Services.Contracts
{
    public interface ICoingeckoApiService
    {
        Task<List<SupportedCoin>> GetSupportedCoins();
        Task<List<SupportedCoin>> GetCoinPrices();
        Task<List<SupportedCoin>> GetCachedCoinPrices();

    }
}
