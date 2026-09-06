namespace Core.Events.Tests;

public class InMemoryEventStoreTests
{
    #region RaiseEventAsync - nominal cases

    [Theory]
    [InlineData(EventType.BlastCreated)]
    [InlineData(EventType.HoleAdded)]
    [InlineData(EventType.HoleCharged)]
    [InlineData(EventType.BlastFired)]
    public async Task RaiseEventAsync_WithNewEventTypeAndAggregateId_DoesNotThrow(EventType eventType)
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();

        // Act
        var exception = await Record.ExceptionAsync(
            () => store.RaiseEventAsync(eventType, aggregateId));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(EventType.BlastCreated, typeof(BlastCreated))]
    [InlineData(EventType.HoleAdded, typeof(HoleAdded))]
    [InlineData(EventType.HoleCharged, typeof(HoleCharged))]
    [InlineData(EventType.BlastFired, typeof(BlastFired))]
    public async Task RaiseEventAsync_AddsCorrespondingDomainEventToStore(EventType eventType, Type expectedDomainEventType)
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();

        // Act
        await store.RaiseEventAsync(eventType, aggregateId);
        var events = (await store.GetEventsByAggregateId(aggregateId)).ToList();

        // Assert
        var storedEvent = Assert.Single(events);
        Assert.IsType(expectedDomainEventType, storedEvent);
        Assert.Equal(aggregateId, storedEvent.AggregateId);
        Assert.Equal(eventType, storedEvent.EventType);
    }

    #endregion

    #region RaiseEventAsync - duplicate detection

    [Theory]
    [InlineData(EventType.BlastCreated)]
    [InlineData(EventType.HoleAdded)]
    [InlineData(EventType.HoleCharged)]
    [InlineData(EventType.BlastFired)]
    public async Task RaiseEventAsync_CalledTwiceWithSameEventTypeAndAggregateId_ThrowsEventStoreExistingException(EventType eventType)
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();
        await store.RaiseEventAsync(eventType, aggregateId);

        // Act & Assert
        await Assert.ThrowsAsync<EventStoreExistingException>(
            () => store.RaiseEventAsync(eventType, aggregateId));
    }

    [Fact]
    public async Task RaiseEventAsync_AfterFailedDuplicateAttempt_StoreStillContainsOnlyOneEvent()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();
        await store.RaiseEventAsync(EventType.BlastCreated, aggregateId);

        // Act
        await Assert.ThrowsAsync<EventStoreExistingException>(
            () => store.RaiseEventAsync(EventType.BlastCreated, aggregateId));
        var events = (await store.GetEventsByAggregateId(aggregateId)).ToList();

        // Assert : la tentative en échec n'a pas ajouté de doublon
        Assert.Single(events);
    }

    [Fact]
    public async Task RaiseEventAsync_CalledTwiceWithSameEventTypeButDifferentAggregateId_DoesNotThrow()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var firstAggregateId = Guid.NewGuid();
        var secondAggregateId = Guid.NewGuid();
        await store.RaiseEventAsync(EventType.BlastCreated, firstAggregateId);

        // Act
        var exception = await Record.ExceptionAsync(
            () => store.RaiseEventAsync(EventType.BlastCreated, secondAggregateId));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task RaiseEventAsync_CalledWithSameAggregateIdButDifferentEventType_DoesNotThrow()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();
        await store.RaiseEventAsync(EventType.BlastCreated, aggregateId);

        // Act
        var exception = await Record.ExceptionAsync(
            () => store.RaiseEventAsync(EventType.HoleAdded, aggregateId));

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region RaiseEventAsync - unhandled event type

    [Fact]
    public async Task RaiseEventAsync_WithUndefinedEventType_ThrowsNotImplementedException()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();
        var undefinedEventType = (EventType)999;

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => store.RaiseEventAsync(undefinedEventType, aggregateId));
    }

    [Fact]
    public async Task RaiseEventAsync_WithUndefinedEventType_DoesNotAddEventToStore()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();
        var undefinedEventType = (EventType)999;

        // Act
        await Assert.ThrowsAsync<NotImplementedException>(
            () => store.RaiseEventAsync(undefinedEventType, aggregateId));
        var events = await store.GetEventsByAggregateId(aggregateId);

        // Assert
        Assert.Empty(events);
    }

    #endregion

    #region GetEventsByAggregateId

    [Fact]
    public async Task GetEventsByAggregateId_WithNoEventsRaised_ReturnsEmptyCollection()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();

        // Act
        var events = await store.GetEventsByAggregateId(aggregateId);

        // Assert
        Assert.Empty(events);
    }

    [Fact]
    public async Task GetEventsByAggregateId_ReturnsOnlyEventsForRequestedAggregate()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var targetAggregateId = Guid.NewGuid();
        var otherAggregateId = Guid.NewGuid();
        await store.RaiseEventAsync(EventType.BlastCreated, targetAggregateId);
        await store.RaiseEventAsync(EventType.HoleAdded, otherAggregateId);

        // Act
        var events = (await store.GetEventsByAggregateId(targetAggregateId)).ToList();

        // Assert
        var storedEvent = Assert.Single(events);
        Assert.Equal(targetAggregateId, storedEvent.AggregateId);
    }

    [Fact]
    public async Task GetEventsByAggregateId_AfterMultipleEventsForSameAggregate_ReturnsAllOfThem()
    {
        // Arrange
        var store = new InMemoryEventStore();
        var aggregateId = Guid.NewGuid();
        await store.RaiseEventAsync(EventType.BlastCreated, aggregateId);
        await store.RaiseEventAsync(EventType.HoleAdded, aggregateId);
        await store.RaiseEventAsync(EventType.HoleCharged, aggregateId);
        await store.RaiseEventAsync(EventType.BlastFired, aggregateId);

        // Act
        var events = (await store.GetEventsByAggregateId(aggregateId)).ToList();

        // Assert
        Assert.Equal(4, events.Count);
        Assert.Contains(events, e => e is BlastCreated);
        Assert.Contains(events, e => e is HoleAdded);
        Assert.Contains(events, e => e is HoleCharged);
        Assert.Contains(events, e => e is BlastFired);
    }

    #endregion
}

public class EventStoreExistingExceptionTests
{
    [Fact]
    public void Message_ReturnsExpectedText()
    {
        // Arrange
        var exception = new EventStoreExistingException();

        // Act & Assert
        Assert.Equal("Event already exists in the event store.", exception.Message);
    }

    [Fact]
    public void EventStoreExistingException_IsAnException()
    {
        // Arrange & Act
        var exception = new EventStoreExistingException();

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}

public class DomainEventTests
{
    [Fact]
    public void BlastCreated_Constructor_SetsExpectedValues()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;

        // Act
        var domainEvent = new BlastCreated(aggregateId);
        var after = DateTimeOffset.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal(EventType.BlastCreated, domainEvent.EventType);
        Assert.Equal("Blast", domainEvent.AggregateType);
        Assert.InRange(domainEvent.OccurredOn, before, after);
    }

    [Fact]
    public void HoleAdded_Constructor_SetsExpectedValues()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;

        // Act
        var domainEvent = new HoleAdded(aggregateId);
        var after = DateTimeOffset.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal(EventType.HoleAdded, domainEvent.EventType);
        Assert.Equal("Hole", domainEvent.AggregateType);
        Assert.InRange(domainEvent.OccurredOn, before, after);
    }

    [Fact]
    public void HoleCharged_Constructor_SetsExpectedValues()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;

        // Act
        var domainEvent = new HoleCharged(aggregateId);
        var after = DateTimeOffset.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal(EventType.HoleCharged, domainEvent.EventType);
        Assert.Equal("Hole", domainEvent.AggregateType);
        Assert.InRange(domainEvent.OccurredOn, before, after);
    }

    [Fact]
    public void BlastFired_Constructor_SetsExpectedValues()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;

        // Act
        var domainEvent = new BlastFired(aggregateId);
        var after = DateTimeOffset.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal(EventType.BlastFired, domainEvent.EventType);
        Assert.Equal("Blast", domainEvent.AggregateType);
        Assert.InRange(domainEvent.OccurredOn, before, after);
    }

    [Fact]
    public void TwoInstances_OfSameEventClass_HaveDifferentEventIds()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();

        // Act
        var first = new BlastCreated(aggregateId);
        var second = new BlastCreated(aggregateId);

        // Assert
        Assert.NotEqual(first.EventId, second.EventId);
    }
}