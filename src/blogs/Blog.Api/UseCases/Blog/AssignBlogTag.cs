namespace Blog.Api.UseCases.Blog;

public struct AssignBlogTag
{
    public record AddTagCommand : ICreateCommand
    {
        public Guid BlogId { get; init; }
        public List<Tag> Tags { get; init; }

        internal class Validator : AbstractValidator<AddTagCommand>
        {
            public Validator()
            {
                RuleFor(x => x.BlogId)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Blog Id is not null");
            }
        }
    }

    public record RemoveTagCommand : ICreateCommand
    {
        public Guid BlogId { get; init; }
        public List<Tag> Tags { get; init; }

        internal class Validator : AbstractValidator<RemoveTagCommand>
        {
            public Validator()
            {
                RuleFor(x => x.BlogId)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Blog Id is not null");
            }
        }
    }

    internal class Handler :
        IRequestHandler<AddTagCommand, IResult>,
        IRequestHandler<RemoveTagCommand, IResult>
    {
        private readonly IBlogRepository _blogRepository;

        public Handler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IResult> Handle(AddTagCommand request, CancellationToken cancellationToken)
        {
            var item = await _blogRepository.GetByIdAsync(request.BlogId);
            if (item is null) throw new Exception($"Couldn't find item={request.BlogId}");
            item.AddTag(request.Tags);
            _blogRepository.Update(item);
            await _blogRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(RemoveTagCommand request, CancellationToken cancellationToken)
        {
            var item = await _blogRepository.GetByIdAsync(request.BlogId);
            if (item is null) throw new Exception($"Couldn't find item={request.BlogId}");
            item.RemoveTag(request.Tags.Select(x => x.Id).ToList());
            _blogRepository.Update(item);
            await _blogRepository.CommitAsync();
            return Results.Ok();
        }
    }
}