using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Bds.TechTest.Domain;

namespace Bds.TechTest.WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchEngineResultsController : ControllerBase
    {
        private readonly ISearchEngineScrapingService _searchEngineScrapingService;

        public SearchEngineResultsController(ISearchEngineScrapingService searchEngineScrapingService)
        {
            _searchEngineScrapingService = searchEngineScrapingService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new BadRequestObjectResult("Search term must not be null or empty.");
            }

            var searchResults = await _searchEngineScrapingService.ScrapeAllEngines(searchTerm);

            return new OkObjectResult(searchResults);
        }
    }
}
