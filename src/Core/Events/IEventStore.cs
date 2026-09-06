namespace Core.Events;

public interface IEventStore
{
    Task<IEnumerable<IDomainEvent>> GetEventsByAggregateId(Guid blastId);
    Task RaiseEventAsync(EventType eventType, Guid aggregateId);
}

public enum EventType
{
    BlastCreated,
    HoleAdded,
    HoleCharged,
    BlastFired,
}

public class InMemoryEventStore : IEventStore
{
    private readonly List<EventRecord> _events = new();

    public Task<IEnumerable<IDomainEvent>> GetEventsByAggregateId(Guid blastId)
    {
        var events = _events
            .Where(e => e.AggregateId == blastId)
            .Select(e => e.DomainEvent);
        return Task.FromResult(events);
    }

    public Task RaiseEventAsync(EventType eventType, Guid aggregateId)
    {
        var existingEvent = _events.FirstOrDefault(e =>
            e.EventType == eventType && e.AggregateId == aggregateId
        );
        if (existingEvent is not null)
        {
            throw new EventStoreExistingException();
        }

        IDomainEvent domainEvent = eventType switch
        {
            EventType.BlastCreated => new BlastCreated(aggregateId),
            EventType.HoleAdded => new HoleAdded(aggregateId),
            EventType.HoleCharged => new HoleCharged(aggregateId),
            EventType.BlastFired => new BlastFired(aggregateId),
            _ => throw new NotImplementedException($"Event type {eventType} is not implemented."),
        };
        _events.Add(new EventRecord(eventType, aggregateId, domainEvent));
        return Task.CompletedTask;
    }
}

internal class EventRecord
{
    public EventType EventType { get; }
    public Guid AggregateId { get; }
    public IDomainEvent DomainEvent { get; }

    public EventRecord(EventType eventType, Guid aggregateId, IDomainEvent domainEvent)
    {
        EventType = eventType;
        AggregateId = aggregateId;
        DomainEvent = domainEvent;
    }
}

public class EventStoreExistingException : Exception
{
    public EventStoreExistingException() { }

    public override string Message => "Event already exists in the event store.";
}
