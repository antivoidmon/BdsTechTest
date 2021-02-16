using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;

namespace Bds.TechTest.Domain
{
    public class YandexScrapingStrategy : ScrapingStrategyBase, IScrapingStrategy
    {
        public YandexScrapingStrategy(Url baseUrl) : base(baseUrl){}

        public IEnumerable<SearchEngineResultValueObject> Scrape(IDocument document)
        {
            var body = document.Body;
            var searchResultsDiv = body.QuerySelectorAll("li.serp-item");

            var searchResultValueObjects = searchResultsDiv.Select(r =>
            {
                var titleElement = r.QuerySelector("h2 .organic__url-text");
                if (titleElement == null) return null;
                var descriptionElement = r.QuerySelector(".text-container");
                if (descriptionElement == null) return null;
                return new SearchEngineResultValueObject("Yandex", 
                    CleanupLineBreaksAndWhitespace(titleElement.TextContent), 
                    CleanupLineBreaksAndWhitespace(descriptionElement.TextContent));
            }).Where(r => r != null);
                
            return searchResultValueObjects;
        }

        public Url BuildSearchUrl(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm)) throw new ArgumentNullException(nameof(searchTerm));

            return new Url(BaseUrl, $"search?text={searchTerm}");
        }

        private string CleanupLineBreaksAndWhitespace(string toBeCleaned)
        {
            var split = toBeCleaned.Split(new []{ "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ",
                split.Select(line => line.Trim())
            );
        }
    }
}