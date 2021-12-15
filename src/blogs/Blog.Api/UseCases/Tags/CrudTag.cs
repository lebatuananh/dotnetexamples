using System.Linq.Expressions;

namespace Blog.Api.UseCases.Tags;

public struct MutateTag
{
    public record GetListTagQueries : IQueries
    {
        public int Skip { get; init; }
        public int Take { get; init; }
        public string? Query { get; init; }
    }

    public record GetTagQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<Tag>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
            }

            public override Expression<Func<Tag, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetTagQuery>
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

    public record CreateTagCommand : ICreateCommand
    {
        public string Name { get; init; }

        public Tag ToBlogEntity()
        {
            return new Tag(Name);
        }

        internal class Validator : AbstractValidator<CreateTagCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Name is not empty");
            }
        }
    }

    public record UpdateTagCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }


        internal class Validator : AbstractValidator<UpdateTagCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Title is not empty");

                RuleFor(x => x.Id)
                    .NotNull()
                    .WithMessage("Id is not null");
            }
        }
    }


    internal class Handler :
        IRequestHandler<GetListTagQueries, IResult>,
        IRequestHandler<GetTagQuery, IResult>,
        IRequestHandler<CreateTagCommand, IResult>,
        IRequestHandler<UpdateTagCommand, IResult>
    {
        private readonly ITagRepository _tagRepository;


        public Handler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IResult> Handle(GetListTagQueries request, CancellationToken cancellationToken)
        {
            var queryable = await _tagRepository
                .FindAll(x => string.IsNullOrEmpty(request.Query)
                              || EF.Functions.ILike(x.Name, $"%{request.Query}%")
                )
                .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
            var tagModels = new QueryResult<TagDto>()
            {
                Count = queryable.Count,
                Items = queryable.Items.Select(x => new TagDto(x.Id, x.Name, x.CreatedDate, x.LastUpdatedDate))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<TagDto>>.Create(tagModels));
        }

        public async Task<IResult> Handle(GetTagQuery request, CancellationToken cancellationToken)
        {
            var item = await _tagRepository.GetByIdAsync(request.Id);
            if (item is null)
            {
                throw new Exception($"Couldn't find item={request.Id}");
            }

            var result = new TagDto(item.Id, item.Name, item.CreatedDate, item.LastUpdatedDate);

            return Results.Ok(ResultModel<TagDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var tagEntity = request.ToBlogEntity();
            _tagRepository.Add(tagEntity);
            await _tagRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            var item = await _tagRepository.GetByIdAsync(request.Id);
            if (item is null)
            {
                throw new Exception($"Couldn't find entity with id={request.Id}");
            }

            item.Update(request.Name);
            _tagRepository.Update(item);
            await _tagRepository.CommitAsync();
            return Results.Ok();
        }
    }
}