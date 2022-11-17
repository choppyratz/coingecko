using Microsoft.Extensions.Caching.Memory;
using CoingeckoParser.Models;
using CoingeckoParser.Models.Enums;
using CoingeckoParser.Services.Contracts;
using Newtonsoft.Json;
using System.Net;

namespace CoingeckoParser.Services
{
    public class CoingeckoApiService : ICoingeckoApiService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly string _coingeckoBaseUri;

        public CoingeckoApiService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _coingeckoBaseUri = "https://api.coingecko.com/api/v3";
            _httpClient = new HttpClient();
        }

        public async Task<List<SupportedCoin>> GetSupportedCoins()
        {
            if (!_memoryCache.TryGetValue(CacheKey.SupportedCoins, out List<SupportedCoin> cacheValue))
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_coingeckoBaseUri}/coins/list");
                var response = await _httpClient.SendAsync(request);

                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    cacheValue = JsonConvert.DeserializeObject<List<SupportedCoin>>(jsonString).Take(500).ToList();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(100));

                _memoryCache.Set(CacheKey.SupportedCoins, cacheValue, cacheEntryOptions);
            }

            return cacheValue;
        }

        public async Task<List<SupportedCoin>> GetCoinPrices()
        {
            var supportedCoinPrices = new List<SupportedCoin>();
            var supportedCoins = await GetSupportedCoins();
            var coinChuncks = supportedCoins.Chunk(400);
            foreach (var coinChunk in coinChuncks)
            {
                var supportedCoinIdsCsv = string.Join(',', coinChunk.Select(item => item.Id).ToList());
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_coingeckoBaseUri}/simple/price?ids={supportedCoinIdsCsv}&vs_currencies=usd,eur,rub");
                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    break;
                }

                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var supportedCoinPricesChunk = JsonConvert.DeserializeObject<Dictionary<string, SupportedCoin>>(jsonString).Select(item =>
                    {
                        var supportedCoin = coinChunk.FirstOrDefault(coin => coin.Id == item.Key);
                        return new SupportedCoin
                        {
                            Id = item.Key,
                            Usd = item.Value.Usd,
                            Eur = item.Value.Eur,
                            Rub = item.Value.Rub,
                            Name = supportedCoin.Name,
                            Symbol = supportedCoin.Symbol
                        };
                    });
                    supportedCoinPrices.AddRange(supportedCoinPricesChunk);
                }
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(100));

            _memoryCache.Set(CacheKey.SupportedCoinPrices, supportedCoinPrices, cacheEntryOptions);
            return supportedCoinPrices;
        }

        public async Task<List<SupportedCoin>> GetCachedCoinPrices()
        {
            if (!_memoryCache.TryGetValue(CacheKey.SupportedCoinPrices, out List<SupportedCoin> cacheValue))
            {
                cacheValue = await GetCoinPrices();
            }
            return cacheValue;
        }
    }
}
