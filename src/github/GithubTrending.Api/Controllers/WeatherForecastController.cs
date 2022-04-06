using DotnetCrawler.Core;
using DotnetCrawler.Downloader;
using DotnetCrawler.Pipeline;
using DotnetCrawler.Processor;
using DotnetCrawler.Request;
using GithubTrending.Domain.AggregateModel.GithubRepositoriesAggregate;
using GithubTrending.Infrastructure;
using GithubTrending.Infrastructure.Repositories;

namespace GithubTrending.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IGithubTrendingRepository _githubTrendingRepository;

    public WeatherForecastController(IGithubTrendingRepository githubTrendingRepository)
    {
        _githubTrendingRepository = githubTrendingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Crawler()
    {
        var crawler = new DotnetCrawler<GithubRepositories>()
            .AddRequest(new DotnetCrawlerRequest
                { Url = "https://www.ebay.com/b/Apple-iPhone/9355/bn_319682", Regex = @".*itm/.+", TimeOut = 5000 })
            .AddDownloader(new DotnetCrawlerDownloader
                { DownloderType = DotnetCrawlerDownloaderType.FromMemory, DownloadPath = @"C:\DotnetCrawlercrawler\" })
            .AddProcessor(new DotnetCrawlerProcessor<GithubRepositories> { })
            .AddPipeline(
                new DotnetCrawlerPipeline<GithubRepositories>(_githubTrendingRepository) { });

        await crawler.Crawle();
        return Ok();
    }
}