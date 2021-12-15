namespace Blog.Api.UseCases.Blog;

public record BlogDto(Guid Id, string Title, string Description, string Poster, string Content, BlogStatus Status,
    DateTimeOffset CreatedDate, DateTimeOffset LastUpdatedDate)
{
    public List<BlogTagDto> BlogTagDtos { get; set; }

    public BlogDto AssignTagNames(IEnumerable<BlogTag> blogTags)
    {
        BlogTagDtos = blogTags.Select(x => new BlogTagDto(x.TagId, x.TagName)).ToList();
        return this;
    }
}

public record BlogTagDto(Guid TagId, string Name);