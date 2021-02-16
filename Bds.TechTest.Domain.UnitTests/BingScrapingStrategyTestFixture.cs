using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using NUnit.Framework;
using Shouldly;

namespace Bds.TechTest.Domain
{
    [TestFixture]
    public class BingScrapingStrategyTestFixture
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        public void Ctor_Given_UrlIsMissing_Then_ArgumentExceptionThrown(string url)
        {
            // Arrange
            var baseUrl = new Url(url);

            // Act, Assert
            var thrown = Should.Throw<ArgumentException>(() => new YandexScrapingStrategy(baseUrl));
            thrown.Message.ShouldStartWith("Must not be empty string.");
            thrown.ParamName.ShouldBe("baseUrl");
        }

        [Test]
        public async Task Scrape_Given_HtmlString_Then_SearchEngineResultsScraped()
        {
            // Arrange
            var document = await GetTestDocument("BingFoobarSearch.html");

            // Act
            var results = GetTestSubject().Scrape(document).ToList();

            // Assert
            var firstResult = results.First();
            firstResult.Title.ShouldBe("foobar2000");
            firstResult.Description.ShouldStartWith("foobar2000 is an advanced freeware audio player for the Windows platform.");

            var secondResult = results[1];
            secondResult.Title.ShouldBe("Foobar - Wikipedia");
            secondResult.Description.ShouldStartWith("The terms foobar , foo, bar, and others are used as metasyntactic variables  and placeholder names  in computer programming  or computer-related documentation.");

            results.Count.ShouldBe(7);

            var lastResult = results[6];
            lastResult.Title.ShouldBe("foobar2000 1.6.4 - Neowin");
            lastResult.Description.ShouldStartWith("01/02/2021 · foobar2000 is an advanced freeware audio player for the Windows platform.");
        }

        [Test]
        public async Task Scrape_Given_HtmlString_Then_SearchEngineResultsShouldNotContainNulls()
        {
            // Arrange
            var htmlDocument = await GetTestDocument("BingFoobarSearch.html");

            // Act
            var results = GetTestSubject().Scrape(htmlDocument);

            // Assert
            results.ShouldNotContain(r => r == null);
        }

        [TestCase("foobar", "foobar", Description = "Standard search term.")]
        [TestCase("foobar & wibble", "foobar%20&%20wibble", Description = "Containing ampersand and space.")]
        public void BuildSearchUrl_Given_SearchTerm_Then_SearchUrlBuiltAndEncoded(string searchTerm, string expectedRelativeAddress)
        {
            // Arrange
            var bingBaseUrl = "https://bing.com";

            // Act
            var actual = GetTestSubject(bingBaseUrl).BuildSearchUrl(searchTerm);

            // Assert
            actual.ToString().ShouldBe($"https://bing.com/search?q={expectedRelativeAddress}");
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void BuildSearchUrl_Given_SearchTermNullOrWhitespace_Then_ArgumentExceptionThrown(string searchTerm)
        {
            // Arrange, Act
            var thrown = Should.Throw<ArgumentException>(() => GetTestSubject().BuildSearchUrl(searchTerm));

            // Assert
            thrown.ParamName.ShouldBe("searchTerm");
            thrown.Message.ShouldStartWith("Must not be null or whitespace.");
        }

        private static async Task<IDocument> GetTestDocument(string fileNameWithExtension)
        {
            var htmlString = await File.ReadAllTextAsync(TestContext.CurrentContext.TestDirectory + $"\\TestData\\{fileNameWithExtension}");
            var browsingContext = BrowsingContext.New();
            return browsingContext.OpenAsync(req => req.Content(htmlString)).Result;
        }

        private IScrapingStrategy GetTestSubject(string baseUrl = "http://bing")
        {
            return new BingScrapingStrategy(new Url(baseUrl));
        }
    }
}