using System;
using AngleSharp.Dom;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;

namespace Bds.TechTest.Domain
{
    public class BingScrapingStrategy : ScrapingStrategyBase, IScrapingStrategy
    {
        public BingScrapingStrategy(Url baseUrl) : base(baseUrl)
        {
        }

        public IEnumerable<SearchEngineResultValueObject> Scrape(IDocument document)
        {
            var body = document.Body;
            var results = body.QuerySelectorAll("#b_results > .b_algo");
            var resultValueObjects = results.Select(r =>
            {
                var titleElement = r.QuerySelector("h2 > a");
                if (titleElement == null) return null;
                var descriptionElement = r.QuerySelector("p");
                if (descriptionElement == null) return null;
                return new SearchEngineResultValueObject("Bing", titleElement.TextContent.Trim(),
                    descriptionElement.TextContent.Trim());
            }).Where(r => r != null);

            return resultValueObjects;
        }

        public Url BuildSearchUrl(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Must not be null or whitespace.", nameof(searchTerm));
            return new Url(BaseUrl, $"/search?q={searchTerm}");
        }
    }
}