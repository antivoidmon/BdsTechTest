using System;
using NUnit.Framework;
using Shouldly;

namespace Bds.TechTest.Domain
{
    [TestFixture]
    public class SearchEngineResultValueObjectTestFixture
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Ctor_Given_TitleIsMissing_Then_ArgumentExceptionThrown(string title)
        {
            // Arrange, Act
            var thrown = Should.Throw<ArgumentException>(() => new SearchEngineResultValueObject("Google", title, "wibble"));

            // Assert
            thrown.Message.ShouldStartWith("Value must not be null or whitespace.");
            thrown.ParamName.ShouldBe("title");
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Ctor_Given_DescriptionIsMissing_Then_ArgumentExceptionThrown(string description)
        {
            // Arrange, Act
            var thrown = Should.Throw<ArgumentException>(() => new SearchEngineResultValueObject("Google", "foobar", description));

            // Assert
            thrown.Message.ShouldStartWith("Value must not be null or whitespace.");
            thrown.ParamName.ShouldBe("description");
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Ctor_Given_SearchEngineIsMissing_Then_ArgumentExceptionThrown(string searchEngine)
        {
            // Arrange, Act
            var thrown = Should.Throw<ArgumentException>(() => new SearchEngineResultValueObject(searchEngine, "foobar", "wibble"));

            // Assert
            thrown.Message.ShouldStartWith("Value must not be null or whitespace.");
            thrown.ParamName.ShouldBe("searchEngine");
        }

        [Test]
        public void Ctor_Given_ParametersNotMissing_Then_ParametersAssigned()
        {
            // Arrange
            var title = "foobar";
            var description = "wibble";
            var searchEngine = "fizzbuzz";

            // Act
            var actual = new SearchEngineResultValueObject(searchEngine, title, description);

            // Assert
            actual.Title.ShouldBe(title);
            actual.Description.ShouldBe(description);
            actual.SearchEngine.ShouldBe(searchEngine);
        }
    }
}
