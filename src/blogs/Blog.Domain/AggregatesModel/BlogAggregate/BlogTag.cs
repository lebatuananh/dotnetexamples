using Blog.Domain.AggregatesModel.TagAggregate;

namespace Blog.Domain.AggregatesModel.BlogAggregate;

public class BlogTag 
{
    public Guid TagId { get; private set; }
    public Guid BlogId { get; private set; }
    public string TagName { get; private set; }
    public virtual Blog Blog { get; private set; }
    public virtual Tag Tag { get; private set; }

    public BlogTag(Guid tagId, Guid blogId)
    {
        TagId = tagId;
        BlogId = blogId;
    }
}