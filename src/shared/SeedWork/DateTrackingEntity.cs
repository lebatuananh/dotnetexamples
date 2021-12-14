using System;

namespace Shared.SeedWork;

public interface IDateTracking
{
    DateTimeOffset CreatedDate { get; }
    DateTimeOffset LastUpdatedDate { get; }

    void MarkCreated();
    void MarkModified();
}

public abstract class DateTrackingEntity : Entity, IDateTracking
{
    public DateTimeOffset CreatedDate { get; private set; }
    public DateTimeOffset LastUpdatedDate { get; private set; }

    public void MarkCreated()
    {
        var now = DateTimeOffset.UtcNow;
        CreatedDate = now;
        LastUpdatedDate = now;
    }

    public void MarkModified()
    {
        LastUpdatedDate = DateTimeOffset.UtcNow;
    }
}