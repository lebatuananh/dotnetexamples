using Blog.Domain.AggregatesModel.BlogAggregate;
using Shared.SeedWork;
using BlogEntity = Blog.Domain.AggregatesModel.BlogAggregate.Blog;

namespace Blog.Infrastructure.Repositories;

public class BlogRepository : Repository<BlogEntity, BlogDbContext>, IBlogRepository
{
    public BlogRepository(BlogDbContext dbContext) : base(dbContext)
    {
    }
}