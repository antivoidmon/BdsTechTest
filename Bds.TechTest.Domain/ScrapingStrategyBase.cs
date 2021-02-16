using System;
using AngleSharp;

namespace Bds.TechTest.Domain
{
    public class ScrapingStrategyBase
    {
        protected readonly Url BaseUrl;

        protected ScrapingStrategyBase(Url baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl.ToString()))
                throw new ArgumentException("Must not be empty string.", nameof(baseUrl));

            BaseUrl = baseUrl;
        }
    }
}
