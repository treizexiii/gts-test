using Core.Models;

namespace Core.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    Guid AggregateId { get; }
    EventType EventType { get; }
    DateTimeOffset OccurredOn { get; }
    string AggregateType { get; }
}

public class BlastCreated : IDomainEvent
{
    public Guid EventId { get; set; }
    public Guid AggregateId { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
    public EventType EventType { get; set; }
    public string AggregateType => nameof(Blast);

    public BlastCreated(Guid id)
    {
        EventId = Guid.NewGuid();
        AggregateId = id;
        OccurredOn = DateTimeOffset.UtcNow;
        EventType = EventType.BlastCreated;
    }
}

public class HoleAdded : IDomainEvent
{
    public Guid EventId { get; set; }
    public Guid AggregateId { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
    public EventType EventType { get; set; }
    public string AggregateType => nameof(Hole);

    public HoleAdded(Guid id)
    {
        EventId = Guid.NewGuid();
        AggregateId = id;
        OccurredOn = DateTimeOffset.UtcNow;
        EventType = EventType.HoleAdded;
    }
}

public class HoleCharged : IDomainEvent
{
    public Guid EventId { get; set; }
    public Guid AggregateId { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
    public EventType EventType { get; set; }
    public string AggregateType => nameof(Hole);

    public HoleCharged(Guid id)
    {
        EventId = Guid.NewGuid();
        AggregateId = id;
        OccurredOn = DateTimeOffset.UtcNow;
        EventType = EventType.HoleCharged;
    }
}

public class BlastFired : IDomainEvent
{
    public Guid EventId { get; set; }
    public Guid AggregateId { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
    public EventType EventType { get; set; }
    public string AggregateType => nameof(Blast);

    public BlastFired(Guid id)
    {
        EventId = Guid.NewGuid();
        AggregateId = id;
        OccurredOn = DateTimeOffset.UtcNow;
        EventType = EventType.BlastFired;
    }
}