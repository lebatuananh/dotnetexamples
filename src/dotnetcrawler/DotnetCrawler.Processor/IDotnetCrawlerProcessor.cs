using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Shared.SeedWork;

namespace DotnetCrawler.Processor
{
    public interface IDotnetCrawlerProcessor<TEntity> where TEntity : class, IAggregateRoot
    {
        Task<IEnumerable<TEntity>> Process(HtmlDocument document);
    }
}
