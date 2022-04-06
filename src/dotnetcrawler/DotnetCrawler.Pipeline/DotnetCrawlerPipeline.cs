using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.SeedWork;

namespace DotnetCrawler.Pipeline
{
    public class DotnetCrawlerPipeline<TEntity> : IDotnetCrawlerPipeline<TEntity>
        where TEntity : Entity, IAggregateRoot
    {
        private readonly IRepository<TEntity> _repository;

        public DotnetCrawlerPipeline(IRepository<TEntity> repository)
        {
            _repository = repository;
        }


        public async Task Run(IEnumerable<TEntity> entityList)
        {
            foreach (var entity in entityList)
            {
                _repository.Add(entity);
                await _repository.CommitAsync();
            }
        }
    }
}