using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bds.TechTest.Domain
{
    public interface ISearchEngineScrapingService
    {
        Task<IEnumerable<SearchEngineResultValueObject>> ScrapeAllEngines(string searchTerm);
    }
}
