using Shared.SeedWork;

namespace Blog.Domain.AggregatesModel.TagAggregate;

public class Tag : ModifierTrackingEntity, IAggregateRoot
{
    public string Name { get; private set; }

    public Tag(string name)
    {
        Name = name;
    }

    public void Update(string name)
    {
        Name = name;
    }
}