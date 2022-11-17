using CoingeckoParser.Models;
using CoingeckoParser.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoingeckoParser.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CoingeckoController : ControllerBase
    {

        private readonly ICoingeckoApiService _coingeckoApiService;

        public CoingeckoController(ICoingeckoApiService coingeckoApiService)
        {
            _coingeckoApiService = coingeckoApiService;
        }
        // GET: api/<CoingeckoController>
        [HttpGet]
        public async Task<IEnumerable<SupportedCoin>> Get()
        {
            return await _coingeckoApiService.GetCachedCoinPrices();
        }

    }
}
