using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;
using Bds.TechTest.Domain;

namespace Bds.TechTest.Infrastructure
{
    public class SearchEngineScrapingServiceAgent : ISearchEngineScrapingServiceAgent
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly IScrapingStrategy _scrapingStrategy;

        public SearchEngineScrapingServiceAgent(IBrowsingContext browsingContext, IScrapingStrategy scrapingStrategy)
        {
            _browsingContext = browsingContext;
            _scrapingStrategy = scrapingStrategy;
        }

        public async Task<IEnumerable<SearchEngineResultValueObject>> ScrapeSearchEngine(string searchTerm)
        {
            var document = await _browsingContext.OpenAsync(_scrapingStrategy.BuildSearchUrl(searchTerm));
            return _scrapingStrategy.Scrape(document);
        }
    }
}