using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Shouldly;

namespace Bds.TechTest.Domain
{
    [TestFixture]
    public class SearchEngineScrapingServiceTestFixture
    {
        private ISearchEngineScrapingServiceAgent _mockYandexSearchEngineScrapingServiceAgent;
        private ISearchEngineScrapingServiceAgent _mockBingSearchEngineScrapingServiceAgent;

        private ISearchEngineScrapingService CreateTestSubject(
            IEnumerable<SearchEngineResultValueObject> yandexSearchResults,
            IEnumerable<SearchEngineResultValueObject> bingSearchResults)
        {
            _mockYandexSearchEngineScrapingServiceAgent = Substitute.For<ISearchEngineScrapingServiceAgent>();
            _mockYandexSearchEngineScrapingServiceAgent.ScrapeSearchEngine(default)
                .ReturnsForAnyArgs(yandexSearchResults);

            _mockBingSearchEngineScrapingServiceAgent = Substitute.For<ISearchEngineScrapingServiceAgent>();
            _mockBingSearchEngineScrapingServiceAgent.ScrapeSearchEngine(default).ReturnsForAnyArgs(bingSearchResults);

            return new SearchEngineScrapingService(_mockYandexSearchEngineScrapingServiceAgent, _mockBingSearchEngineScrapingServiceAgent);
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
            var yandexResults = new List<SearchEngineResultValueObject>
            {
                new SearchEngineResultValueObject("Yandex", "Foobar", "Foobar & Wibble"),
                new SearchEngineResultValueObject("Yandex", "Fizz", "Fizz & Buzz")
            };
            var bingResults = new List<SearchEngineResultValueObject>
            {
                new SearchEngineResultValueObject("Bing", "Bing Foobar", "Foobar & Wibble"),
                new SearchEngineResultValueObject("Bing", "Bing Fizz", "Fizz & Buzz")
            };

            var testSubject = CreateTestSubject(yandexResults, bingResults);

            // Act
            var actual = (await testSubject.ScrapeAllEngines("foobar")).ToList();

            // Assert
            actual.Count.ShouldBe(4);
            actual.ShouldContain(r => r.Title == yandexResults.First().Title);
            actual.ShouldContain(r => r.Title == yandexResults.Last().Title);
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

        [Test]
        public async Task ScrapeAllEngines_Given_SearchEngineScrapeThrows_Then_OtherEnginesStillScraped()
        {
            // Arrange
            var yandexResults = new List<SearchEngineResultValueObject>
            {
                new SearchEngineResultValueObject("yandex", "Foobar", "Foobar & Wibble"),
                new SearchEngineResultValueObject("yandex", "Fizz", "Fizz & Buzz")
            };
            
            var testSubject = CreateTestSubject(yandexResults, new List<SearchEngineResultValueObject>());
            _mockBingSearchEngineScrapingServiceAgent.ScrapeSearchEngine(default)
                .ThrowsForAnyArgs(new Exception("Kaboom!"));

            // Act
            var actual = (await testSubject.ScrapeAllEngines("foobar")).ToList();
            
            // Assert
            actual.Count.ShouldBe(2);
            actual.ShouldContain(r => r.Title == yandexResults.First().Title);
            actual.ShouldContain(r => r.Title == yandexResults.Last().Title);
        }
    }
}
