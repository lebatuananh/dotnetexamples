using System.Linq.Expressions;
using BlogEntity = Blog.Domain.AggregatesModel.BlogAggregate.Blog;

namespace Blog.Api.UseCases.Blog;

public struct MutateBlog
{
    public record GetListBlogQueries : IQueries
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public string? Query { get; init; }
    }

    public record GetBlogQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<BlogEntity>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
                Includes.Add(x => x.BlogTags);
            }

            public override Expression<Func<BlogEntity, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetBlogQuery>
            {
                public GetValidator()
                {
                    RuleFor(v => v.Id)
                        .NotEmpty()
                        .WithMessage("Id is required.");
                }
            }
        }
    }

    public record CreateBlogCommand : ICreateCommand
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string Poster { get; init; }
        public string Content { get; init; }
        public BlogStatus Status { get; init; }

        public BlogEntity ToBlogEntity()
        {
            return new BlogEntity(Title, Description, Poster, Content, Status);
        }

        internal class Validator : AbstractValidator<CreateBlogCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty()
                    .WithMessage("Title is not empty");
            }
        }
    }

    public record UpdateBlogCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Poster { get; init; }
        public string Content { get; init; }
        public BlogStatus Status { get; init; }

        internal class Validator : AbstractValidator<UpdateBlogCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty()
                    .WithMessage("Title is not empty");

                RuleFor(x => x.Id)
                    .NotNull()
                    .WithMessage("Id is not null");
            }
        }
    }

    internal class Handler :
        IRequestHandler<GetListBlogQueries, IResult>,
        IRequestHandler<GetBlogQuery, IResult>,
        IRequestHandler<CreateBlogCommand, IResult>,
        IRequestHandler<UpdateBlogCommand, IResult>
    {
        private readonly IBlogRepository _blogRepository;

        public Handler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IResult> Handle(GetListBlogQueries request, CancellationToken cancellationToken)
        {
            var queryable = await _blogRepository
                .FindAll(x => string.IsNullOrEmpty(request.Query)
                              || EF.Functions.ILike(x.Title, $"%{request.Query}%")
                )
                .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Page, request.PageSize);
            var blogModels = new QueryResult<BlogDto>()
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new BlogDto(x.Id, x.Title, x.Description, x.Poster, x.Content, x.Status, x.CreatedDate,
                        x.LastUpdatedDate))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<BlogDto>>.Create(blogModels));
        }

        public async Task<IResult> Handle(GetBlogQuery request, CancellationToken cancellationToken)
        {
            var item = await _blogRepository.GetByIdAsync(request.Id);
            if (item is null)
            {
                throw new Exception($"Couldn't find item={request.Id}");
            }

            var result = new BlogDto(item.Id, item.Title, item.Description, item.Poster, item.Content, item.Status,
                item.CreatedDate,
                item.LastUpdatedDate);
            if (item.BlogTags is { Count: > 0 })
            {
                result.AssignTagNames(item.BlogTags);
            }

            return Results.Ok(ResultModel<BlogDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var blogEntity = request.ToBlogEntity();
            _blogRepository.Add(blogEntity);
            await _blogRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            var item = await _blogRepository.GetByIdAsync(request.Id);
            if (item is null)
            {
                throw new Exception($"Couldn't find entity with id={request.Id}");
            }

            item.Update(request.Title, request.Description, request.Poster, request.Content, request.Status);
            _blogRepository.Update(item);
            await _blogRepository.CommitAsync();
            return Results.Ok();
        }
    }
}