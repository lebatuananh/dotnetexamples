using Blog.Domain.AggregatesModel.TagAggregate;

namespace Blog.Domain.AggregatesModel.BlogAggregate;

public class BlogTag
{
    public BlogTag(Guid tagId, Guid blogId)
    {
        TagId = tagId;
        BlogId = blogId;
    }

    public Guid TagId { get; }
    public Guid BlogId { get; }
    public string TagName { get; private set; }
    public virtual Blog Blog { get; private set; }
    public virtual Tag Tag { get; private set; }
}