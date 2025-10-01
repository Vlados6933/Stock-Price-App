using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockPriceApp.Models;
using StockPriceApp.Services;
using System.Globalization;
using System.Threading.Tasks;

namespace StockPriceApp.Controllers
{
    public class TradeController : Controller
    {
        private readonly IOptions<TradingOptions> _tradingOptions;
        private readonly FinnhubService _finnhubService;
        private readonly IConfiguration _configuration;

        public TradeController(FinnhubService finnhubService, IOptions<TradingOptions> tradingOptions, IConfiguration configuration)
        {
            _finnhubService = finnhubService;
            _tradingOptions = tradingOptions;
            _configuration = configuration;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            if (_tradingOptions.Value.DefaultStockSymbol == null) _tradingOptions.Value.DefaultStockSymbol = "MSFT";


            Dictionary<string, object>? priceQuoteDictionary = await _finnhubService.GetStockPriceQuote(_tradingOptions.Value.DefaultStockSymbol);

            Dictionary<string, object>? companyProfileDictionary = await _finnhubService.GetCompanyProfile(_tradingOptions.Value.DefaultStockSymbol);

            StockTrade stockTrade = new StockTrade() { StockSymbol = _tradingOptions.Value.DefaultStockSymbol };

            if (priceQuoteDictionary != null && companyProfileDictionary != null)
            {
                stockTrade = new StockTrade()
                {
                    StockSymbol = Convert.ToString(companyProfileDictionary["ticker"]),
                    StockName = Convert.ToString(companyProfileDictionary["name"]),
                    Price = Convert.ToDouble(priceQuoteDictionary["c"].ToString(), CultureInfo.InvariantCulture)
                };
            }

            ViewBag.FinnhubToken = _configuration["FinnhubToken"];

            return View(stockTrade);
        }
    }
}
