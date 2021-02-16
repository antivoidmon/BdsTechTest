using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Dom;

namespace Bds.TechTest.Domain
{
    public interface IScrapingStrategy
    {
        IEnumerable<SearchEngineResultValueObject> Scrape(IDocument document);
        Url BuildSearchUrl(string searchTerm);
    }
}