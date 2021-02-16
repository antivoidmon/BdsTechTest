using AngleSharp;
using AngleSharp.Io;
using Bds.TechTest.Domain;
using Bds.TechTest.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Bds.TechTest.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureScrapingServices(services);
        }

        private void ConfigureScrapingServices(IServiceCollection services)
        {
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var yandexBrowsingContext = BrowsingContext.New(config);
            var bingBrowsingContext = BrowsingContext.New(config);

            var searchEngineUrlConfig = Configuration.GetSection("SearchEngineUrls");
            var yandexSearchEngineScrapingServiceAgent =
                new SearchEngineScrapingServiceAgent(yandexBrowsingContext, new YandexScrapingStrategy(new Url(searchEngineUrlConfig["Yandex"])));
            var bingSearchEngineScrapingServiceAgent =
                new SearchEngineScrapingServiceAgent(bingBrowsingContext, new BingScrapingStrategy(new Url(searchEngineUrlConfig["Bing"])));
            var searchEngineScrapingService = new SearchEngineScrapingService(yandexSearchEngineScrapingServiceAgent, bingSearchEngineScrapingServiceAgent);
            services.AddSingleton<ISearchEngineScrapingService>(searchEngineScrapingService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
