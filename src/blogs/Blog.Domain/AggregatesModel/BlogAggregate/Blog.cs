using Blog.Domain.AggregatesModel.TagAggregate;
using Shared.Extensions;
using Shared.SeedWork;

namespace Blog.Domain.AggregatesModel.BlogAggregate;

public class Blog : ModifierTrackingEntity, IAggregateRoot
{
    public Blog(string title, string description, string poster, string content, BlogStatus status)
    {
        Title = title;
        Description = description;
        Poster = poster;
        Content = content;
        Status = status;
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Poster { get; private set; }
    public string Content { get; private set; }
    public BlogStatus Status { get; private set; }

    public virtual ISet<BlogTag> BlogTags { get; private set; }

    public void Update(string title, string description, string poster, string content, BlogStatus status)
    {
        Title = title;
        Description = description;
        Poster = poster;
        Content = content;
        Status = status;
    }

    public void AddTag(IList<Tag> tags)
    {
        BlogTags ??= new HashSet<BlogTag>();
        var blogTags = tags.Select(x => new BlogTag(x.Id, Id));
        BlogTags.Set(blogTags);
    }

    public void RemoveTag(IList<Guid> tagIds)
    {
        BlogTags ??= new HashSet<BlogTag>();
        var blogTags = BlogTags.Where(x => tagIds.Contains(x.TagId));
        BlogTags.RemoveRange(blogTags);
    }
}