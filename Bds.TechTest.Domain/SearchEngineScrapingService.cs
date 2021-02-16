using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bds.TechTest.Domain
{
    public class SearchEngineScrapingService : ISearchEngineScrapingService
    {
        private readonly ISearchEngineScrapingServiceAgent[] _searchEnginesToScrape;

        public SearchEngineScrapingService(params ISearchEngineScrapingServiceAgent[] searchEnginesToScrape)
        {
            if (searchEnginesToScrape == null) throw new ArgumentNullException(nameof(searchEnginesToScrape), "Search engine must not be null.");
            if (searchEnginesToScrape.Length == 0) throw new ArgumentException("You must pass at least 1 search engine to scrape.", nameof(searchEnginesToScrape));
            _searchEnginesToScrape = searchEnginesToScrape;
        }

        public async Task<IEnumerable<SearchEngineResultValueObject>> ScrapeAllEngines(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm)) throw new ArgumentException("Value must not be null or whitespace.", nameof(searchTerm));

            var searchEngineScrapes = 
                _searchEnginesToScrape
                    .Select(searchEngineScrapingServiceAgent => searchEngineScrapingServiceAgent.ScrapeSearchEngine(searchTerm))
                    .ToList();

            var results = await Task.WhenAll(searchEngineScrapes);

            return results.SelectMany(r => r);
        }

    }
}
