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
    public class YandexScrapingStrategyTestFixture
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
            var document = await GetTestDocument("YandexFoobarSearch.html");

            // Act
            var results = GetTestSubject().Scrape(document).ToList();

            // Assert
            var firstResult = results.First();
            firstResult.Title.ShouldBe("foobar2000");
            firstResult.Description.ShouldStartWith("foobar2000 is an advanced freeware audio player for the Windows platform.");

            var secondResult = results[1];
            secondResult.Title.ShouldBe("Download foobar2000");
            secondResult.Description.ShouldStartWith("Download foobar2000 v1.6.4 Read foobar2000 v1.6 release notes.");

            results.Count.ShouldBe(10);

            var lastResult = results[9];
            lastResult.Title.ShouldStartWith("foobar2000 — ВКонтакте");
            lastResult.Description.ShouldStartWith("Subscribers:");
        }

        [Test]
        public async Task Scrape_Given_HtmlString_Then_SearchEngineResultsShouldNotContainNulls()
        {
            // Arrange
            var htmlDocument = await GetTestDocument("YandexFoobarSearch.html");

            // Act
            var results = GetTestSubject().Scrape(htmlDocument).ToList();

            // Assert
            results.ShouldNotContain(r => r == null);
        }

        [TestCase("foobar", "foobar", Description = "Standard search term.")]
        [TestCase("foobar & wibble", "foobar%20&%20wibble", Description = "Containing ampersand and space.")]
        public void BuildSearchUrl_Given_SearchTerm_Then_SearchUrlBuiltAndEncoded(string searchTerm, string expectedRelativeAddress)
        {
            // Arrange
            var yandexBaseUrl = "https://yandex.com";

            // Act
            var actual = GetTestSubject(yandexBaseUrl).BuildSearchUrl(searchTerm);

            // Assert
            actual.ToString().ShouldBe($"https://yandex.com/search?text={expectedRelativeAddress}");
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void BuildSearchUrl_Given_SearchTermNullOrWhitespace_Then_ArgumentExceptionThrown(string searchTerm)
        {
            // Arrange, Act
            var thrown = Should.Throw<ArgumentNullException>(() => GetTestSubject().BuildSearchUrl(searchTerm));

            // Assert
            thrown.ParamName.ShouldBe("searchTerm");
        }

        private static async Task<IDocument> GetTestDocument(string fileNameWithExtension)
        {
            var htmlString = await File.ReadAllTextAsync(TestContext.CurrentContext.TestDirectory + $"\\TestData\\{fileNameWithExtension}");
            var browsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            return browsingContext.OpenAsync(req => req.Content(htmlString)).Result;
        }
        
        private IScrapingStrategy GetTestSubject(string baseUrl = "http://localhost")
        {
            return new YandexScrapingStrategy(new Url(baseUrl));
        }
    }
}