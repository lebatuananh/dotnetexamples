using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.SeedWork;

namespace DotnetCrawler.Pipeline
{
    public interface IDotnetCrawlerPipeline<TEntity> where TEntity : Entity, IAggregateRoot
    {
        Task Run(IEnumerable<TEntity> entity);
    }
}