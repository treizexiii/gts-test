using Core.Events;
using Core.Models;
using Core.Repositories;

namespace QueryTests;

public class QueryTests
{
    [Fact]
    public async Task GetBlastQuery_WhenBlastDoesNotExist_ThrowsArgumentException()
    {
        var fixture = new QueryFixture();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Query.GetBlastQuery(Guid.NewGuid())
        );

        Assert.Equal("blastId", exception.ParamName);
    }

    [Fact]
    public async Task GetBlastQuery_ReturnsBlastAndItsHolesAsDtos()
    {
        var fixture = new QueryFixture();
        var blast = new Blast
        {
            Id = Guid.NewGuid(),
            Name = "Blast 1",
            Status = Status.Blasted,
            DateBlasted = DateTimeOffset.UtcNow,
        };
        var hole = new Hole
        {
            Id = Guid.NewGuid(),
            BlastId = blast.Id,
            Name = "Hole 1",
            Position = new Position { X = 1, Y = 2, Z = 3 },
            Direction = 45,
            Inclination = -10,
            Status = Status.Charged,
        };
        await fixture.Blasts.AddAsync(blast);
        await fixture.Holes.AddAsync(hole);

        var result = await fixture.Query.GetBlastQuery(blast.Id);

        Assert.NotNull(result);
        Assert.Equal(blast.Id, result.Id);
        Assert.Equal("Blast 1", result.Name);
        Assert.Equal("Blasted", result.Status);
        Assert.Equal(blast.DateBlasted, result.DateBlasted);
        var holeDto = Assert.Single(result.Holes);
        Assert.Equal(hole.Id, holeDto.Id);
        Assert.Equal("Hole 1", holeDto.Name);
        Assert.Equal(1, holeDto.Position.X);
        Assert.Equal(2, holeDto.Position.Y);
        Assert.Equal(3, holeDto.Position.Z);
        Assert.Equal(45, holeDto.Direction);
        Assert.Equal(-10, holeDto.Inclination);
        Assert.Equal("Charged", holeDto.Status);
    }

    [Fact]
    public async Task GetBlastQuery_WhenBlastHasNoHoles_ReturnsEmptyHoleList()
    {
        var fixture = new QueryFixture();
        var blast = await fixture.CreateBlastAsync();

        var result = await fixture.Query.GetBlastQuery(blast.Id);

        Assert.NotNull(result);
        Assert.Empty(result.Holes);
    }

    [Fact]
    public async Task GetBlastHistoryQuery_WhenBlastDoesNotExist_ThrowsArgumentException()
    {
        var fixture = new QueryFixture();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Query.GetBlastHistoryQuery(Guid.NewGuid())
        );

        Assert.Equal("blastId", exception.ParamName);
    }

    [Fact]
    public async Task GetBlastHistoryQuery_ReturnsBlastAndHoleEvents()
    {
        var fixture = new QueryFixture();
        var blast = await fixture.CreateBlastAsync();
        var hole = new Hole { Id = Guid.NewGuid(), BlastId = blast.Id, Name = "Hole 1" };
        await fixture.Holes.AddAsync(hole);
        await fixture.Events.RaiseEventAsync(EventType.HoleAdded, hole.Id);
        await fixture.Events.RaiseEventAsync(EventType.BlastFired, blast.Id);

        var result = await fixture.Query.GetBlastHistoryQuery(blast.Id);

        Assert.Equal(3, result.Count);
        Assert.Equal(
            new[] { "BlastCreated", "HoleAdded", "BlastFired" },
            result.Select(domainEvent => domainEvent.EventType)
        );
        Assert.Contains(result, domainEvent =>
            domainEvent.EventType == "BlastCreated" && domainEvent.AggregateId == blast.Id
        );
        Assert.Contains(result, domainEvent =>
            domainEvent.EventType == "HoleAdded" && domainEvent.AggregateId == hole.Id
        );
        Assert.All(result, domainEvent => Assert.False(domainEvent.Id == Guid.Empty));
    }

    private sealed class QueryFixture
    {
        public InMemoryBlastRepository Blasts { get; } = new();
        public InMemoryHolesRepository Holes { get; } = new();
        public InMemoryEventStore Events { get; } = new();
        public Query.Query Query { get; }

        public QueryFixture()
        {
            Query = new Query.Query(Blasts, Holes, Events);
        }

        public async Task<Blast> CreateBlastAsync()
        {
            var blast = new Blast { Id = Guid.NewGuid(), Name = "Blast 1" };
            await Blasts.AddAsync(blast);
            await Events.RaiseEventAsync(EventType.BlastCreated, blast.Id);
            return blast;
        }

    }
}
