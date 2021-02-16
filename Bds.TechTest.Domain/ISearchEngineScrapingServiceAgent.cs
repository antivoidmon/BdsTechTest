using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bds.TechTest.Domain
{
    public interface ISearchEngineScrapingServiceAgent
    {
        Task<IEnumerable<SearchEngineResultValueObject>> ScrapeSearchEngine(string searchTerm);
    }
}
