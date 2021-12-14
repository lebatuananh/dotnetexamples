using Blog.Domain.AggregatesModel.TagAggregate;
using Shared.SeedWork;

namespace Blog.Infrastructure.Repositories;

public class TagRepository: Repository<Tag, BlogDbContext>, ITagRepository
{
    public TagRepository(BlogDbContext dbContext) : base(dbContext)
    {
    }
}