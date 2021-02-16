using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Bds.TechTest.Domain
{
    [TestFixture]
    public class SearchEngineScrapingServiceTestFixture
    {
        private ISearchEngineScrapingServiceAgent _mockYahooSearchEngineScrapingServiceAgent;
        private ISearchEngineScrapingServiceAgent _mockBingSearchEngineScrapingServiceAgent;

        private ISearchEngineScrapingService CreateTestSubject(
            IEnumerable<SearchEngineResultValueObject> yahooSearchResults,
            IEnumerable<SearchEngineResultValueObject> bingSearchResults)
        {
            _mockYahooSearchEngineScrapingServiceAgent = Substitute.For<ISearchEngineScrapingServiceAgent>();
            _mockYahooSearchEngineScrapingServiceAgent.ScrapeSearchEngine(default)
                .ReturnsForAnyArgs(yahooSearchResults);

            _mockBingSearchEngineScrapingServiceAgent = Substitute.For<ISearchEngineScrapingServiceAgent>();
            _mockBingSearchEngineScrapingServiceAgent.ScrapeSearchEngine(default).ReturnsForAnyArgs(bingSearchResults);

            return new SearchEngineScrapingService(_mockYahooSearchEngineScrapingServiceAgent, _mockBingSearchEngineScrapingServiceAgent);
        }

        [Test]
        public void Ctor_Given_NoSearchEnginesProvided_Then_ArgumentNullExceptionThrown()
        {
            // Arrange, Assert
            var thrown = Should.Throw<ArgumentException>(() => new SearchEngineScrapingService());

            // Assert
            thrown.Message.ShouldStartWith("You must pass at least 1 search engine to scrape.");
            thrown.ParamName.ShouldBe("searchEnginesToScrape");
        }

        [Test]
        public void Ctor_Given_NullSearchEngineProvided_Then_ArgumentNullExceptionThrown()
        {
            // Arrange, Assert
            var thrown = Should.Throw<ArgumentNullException>(() => new SearchEngineScrapingService(null));

            // Assert
            thrown.Message.ShouldStartWith("Search engine must not be null.");
            thrown.ParamName.ShouldBe("searchEnginesToScrape");
        }

        [Test]
        public async Task ScrapeAllEngines_Given_MultipleSearchEngines_Then_CombinedResultsReturned()
        {
            // Arrange
            var yahooResults = new List<SearchEngineResultValueObject>
            {
                new SearchEngineResultValueObject("Yahoo", "Foobar", "Foobar & Wibble"),
                new SearchEngineResultValueObject("Yahoo", "Fizz", "Fizz & Buzz")
            };
            var bingResults = new List<SearchEngineResultValueObject>
            {
                new SearchEngineResultValueObject("Bing", "Bing Foobar", "Foobar & Wibble"),
                new SearchEngineResultValueObject("Bing", "Bing Fizz", "Fizz & Buzz")
            };

            var testSubject = CreateTestSubject(yahooResults, bingResults);

            // Act
            var actual = (await testSubject.ScrapeAllEngines("foobar")).ToList();

            // Assert
            actual.Count.ShouldBe(4);
            actual.ShouldContain(r => r.Title == yahooResults.First().Title);
            actual.ShouldContain(r => r.Title == yahooResults.Last().Title);
            actual.ShouldContain(r => r.Title == bingResults.First().Title);
            actual.ShouldContain(r => r.Title == bingResults.Last().Title);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public async Task ScrapeAllEngines_Given_SearchTermIsMissing_Then_ArgumentExceptionThrown(string searchTerm)
        {
            // Arrange, Act
            var thrown =
                await Should.ThrowAsync<ArgumentException>(() => CreateTestSubject(null, null).ScrapeAllEngines(searchTerm));

            // Assert
            thrown.ParamName.ShouldBe("searchTerm");
            thrown.Message.ShouldStartWith("Value must not be null or whitespace.");
        }
    }
}
