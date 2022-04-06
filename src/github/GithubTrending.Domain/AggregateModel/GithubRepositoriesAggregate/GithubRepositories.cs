using DotnetCrawler.Data.Attributes;
using Shared.SeedWork;

namespace GithubTrending.Domain.AggregateModel.GithubRepositoriesAggregate;

[DotnetCrawlerEntity(XPath = "//*[@id='LeftSummaryPanel']/div[1]")]
public class GithubRepositories : ModifierTrackingEntity, IAggregateRoot
{
    public string? Owner { get; set; }
    public string? Repository { get; set; }
    [DotnetCrawlerField(Expression = "//*[@id='itemTitle']/text()", SelectorType = SelectorType.XPath)]
    public string? Language { get; set; }
    public string? Description { get; set; }
    public string? StarCount { get; set; }
    public string? ForkCount { get; set; }
    public string? TodayStarCount { get; set; }
    public string? OwnersTwitterAccount { get; set; }
    public string? Url { get; set; }
    public StatusGithubTrending StatusGithubTrending { get; set; }

    public GithubRepositories()
    {
        
    }
    public GithubRepositories(string? owner, string? repository, string? language, string? description,
        string? starCount, string? forkCount, string? todayStarCount, string? ownersTwitterAccount, string? url)
    {
        Owner = owner;
        Repository = repository;
        Language = language;
        Description = description;
        StarCount = starCount;
        ForkCount = forkCount;
        TodayStarCount = todayStarCount;
        OwnersTwitterAccount = ownersTwitterAccount;
        Url = url;
        StatusGithubTrending = StatusGithubTrending.Created;
    }

    public void Publish()
    {
        StatusGithubTrending = StatusGithubTrending.Published;
    }

    public void Delete()
    {
        StatusGithubTrending = StatusGithubTrending.Deleted;
    }


    public void Update(string? owner, string? repository, string? language, string? description,
        string? starCount, string? forkCount, string? todayStarCount, string? ownersTwitterAccount, string? url)
    {
        Owner = owner;
        Repository = repository;
        Language = language;
        Description = description;
        StarCount = starCount;
        ForkCount = forkCount;
        TodayStarCount = todayStarCount;
        OwnersTwitterAccount = ownersTwitterAccount;
        Url = url;
        StatusGithubTrending = StatusGithubTrending.Updated;
    }
}