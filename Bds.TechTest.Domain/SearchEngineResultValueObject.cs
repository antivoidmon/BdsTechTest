using System;

namespace Bds.TechTest.Domain
{
    public class SearchEngineResultValueObject
    {
        public SearchEngineResultValueObject(string searchEngine, string title, string description)
        {
            Title = !string.IsNullOrWhiteSpace(title) ? title : throw new ArgumentException("Value must not be null or whitespace.", nameof(title));
            Description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentException("Value must not be null or whitespace.", nameof(description));
            SearchEngine = !string.IsNullOrWhiteSpace(searchEngine) ? searchEngine : throw new ArgumentException("Value must not be null or whitespace.", nameof(searchEngine));
        }
        public string Title { get; }
        public string Description { get; }
        public string SearchEngine { get; }
    }
}
