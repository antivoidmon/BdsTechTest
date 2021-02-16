# Overview

I have built an API in dotnet core that scrapes from 2 search engines; Yandex & Bing. I aim to showcase my understanding of clean architecture and design patterns as well as good API design. I very much enjoyed this exercise and search engine scraping is not something I have ever done before so required some learning on my part. I will go through and overview of the design followed by challenges I faced and recommendations on how I would overcome these challenges if we were writing a production ready piece of software.

## How To run
Run the API project and then perform a GET to https://localhost:44364/SearchEngineResults?searchTerm=foobar and replace foobar with your search term.

# Design

## Solution Layout
### Domain Layer
I have laid the solution out based on an "Hexagonal" design with the domain layer at the core. This allows business logic and domain concepts to represented richly in a domain model alongside the structure of an design patterns used. I am a big fan of using Domain-Driven Design to model business domains and the hexagonal layout fits this very well.

### Application Layer
This is the entry point into the app. It has resposibilities of managing API requests (any auth etc. would sit here) and also manages the DI of the app via the standard dotnet core IoC container.

### Infrastructure Layer
This infrastructure layer holds the service agent which makes required requests to fetch document data from search engines. This uses [AngleSharp library](https://github.com/AngleSharp/AngleSharp) to browse documents and parse HTML (as well as other "<>" based markups). It allows for easy querying of the DOM using CSS which is what I have used in my strategies for the 2 search engines.

# Design Concepts

I have used the strategy pattern for selecting which scraping strategy to use based on which search engines are to be scraped. This allows for configuration of which search engines to scrape at run time and allows for easy extension when adding new search engines.

## REST
Whilst there is only a single route on the API it does follow REST convention. The resource is SearchEngineResults and the filter can be passed in the query string.

This should allow for easy extension if we wanted to add paging and field selection to determine the content of the response.

## Test-Driven Development
I have used a TDD approach, this works very well with the hexagonal architecture as it allows for easy mocking of any infrastrtucture types as the interfaces for them live within the Domain.

I have used the following libraries:
- [Shouldy Assertions](https://github.com/shouldly/shouldly)
- [NSubstitute Mocking Library](https://nsubstitute.github.io/)
- [NUnit Testing Framework](https://nunit.org/)


# Challenges

## "Robot Detection"
Particularly with the Yandex after a couple of scrapes they detect that it may be a robot and force a Captcha. A couple of things I would like to add to avoid try and avoid this:

- Changing the UserAgent per request - I'd like to create the BrowsingContext from AngleSharp in a via a BrowsingContextFactory where we can set the default HttpRequestor with a different UserAgent pulled randomly from a pool of UserAgent strings.
- Rotating IP - Again, this would be possible with a proxy which can be configured through AngleSharp.
- Clearing Cookies - This would be required when changing IP and can be configured through AngleSharp.

## DOM Parsing
The DOM can become quite complicated as the search engines tend to serve up the information in different styles. For e.g. Google has a different result markup for WikiPedia results that is more user friendly. This makes it harder to pull out various parts of data as each of these different markups would need supporting.

This could be incorporated into the strategies for each search engine and they can be as comprehensive as needed.

## Cookie Consent
Some search engines will force a cookie consent or some other DOM element that must be navigated before being able to submit a search. In order to get around this, the document retrieval could be added as part of the Search Engine strategy object. So rather than just returning the content from the search url we can interact with the page and confirm for e.g. the cookie consent in order to get back the search results document specific to each search engine.