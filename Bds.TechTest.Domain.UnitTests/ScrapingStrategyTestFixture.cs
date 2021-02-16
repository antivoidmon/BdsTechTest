using System;
using AngleSharp;
using NUnit.Framework;
using Shouldly;

namespace Bds.TechTest.Domain
{
    [TestFixture]
    public class ScrapingStrategyTestFixture
    {
        private class MockScrapingStrategyBase : ScrapingStrategyBase
        {
            public MockScrapingStrategyBase(Url baseUrl) : base(baseUrl)
            {
            }

            public Url GetBaseUrl()
            {
                return BaseUrl;
            }
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        public void Ctor_Given_UrlIsMissing_Then_ArgumentExceptionThrown(string url)
        {
            // Arrange
            var baseUrl = new Url(url);

            // Act, Assert
            var thrown = Should.Throw<ArgumentException>(() => new MockScrapingStrategyBase(baseUrl));
            thrown.Message.ShouldStartWith("Must not be empty string.");
            thrown.ParamName.ShouldBe("baseUrl");
        }

        [Test]
        public void Ctor_Given_BaseUrlIsNotMissing_Then_BaseUrlAssigned()
        {
            // Arrange
            var url = "foobar";

            // Act
            var testSubject = new MockScrapingStrategyBase(new Url(url));

            // Assert
            testSubject.GetBaseUrl().ToString().ShouldBe(url);
        }
    }
}
