namespace Blog.Api.UseCases.Tags;

public record TagDto(Guid Id, string Name, DateTimeOffset CreatedDate, DateTimeOffset LastUpdatedDate);