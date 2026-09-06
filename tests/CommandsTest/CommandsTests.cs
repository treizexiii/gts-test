using Commands;
using Commands.Dto;
using Core.Events;
using Core.Models;
using Core.Repositories;

namespace CommandsTest;

public class CommandsTests
{
    [Fact]
    public async Task CreateBlastCommand_CreatesPlannedBlastAndRaisesEvent()
    {
        var fixture = new CommandsFixture();

        var blastId = await fixture.Commands.CreateBlastComand(
            new CreateBlastDto { Name = "Blast 1" }
        );

        var blast = await fixture.Blasts.GetByIdAsync(blastId);
        var events = await fixture.Events.GetEventsByAggregateId(blastId);

        Assert.NotNull(blast);
        Assert.Equal("Blast 1", blast.Name);
        Assert.Equal(Status.Planned, blast.Status);
        var domainEvent = Assert.Single(events);
        Assert.Equal(EventType.BlastCreated, domainEvent.EventType);
        Assert.Equal(blastId, domainEvent.AggregateId);
    }

    [Fact]
    public async Task AddHoleCommand_WhenBlastDoesNotExist_ThrowsArgumentException()
    {
        var fixture = new CommandsFixture();
        var blastId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Commands.AddHole(blastId, CreateHoleDto())
        );

        Assert.Equal("blastId", exception.ParamName);
    }

    [Fact]
    public async Task AddHoleCommand_CreatesPlannedHoleAndRaisesEvent()
    {
        var fixture = new CommandsFixture();
        var blastId = await fixture.CreateBlastAsync();

        await fixture.Commands.AddHole(blastId, CreateHoleDto());

        var hole = Assert.Single(await fixture.Holes.GetByBlastIdAsync(blastId));
        var events = await fixture.Events.GetEventsByAggregateId(hole.Id);

        Assert.Equal("Hole 1", hole.Name);
        Assert.Equal(1, hole.Position.X);
        Assert.Equal(2, hole.Position.Y);
        Assert.Equal(3, hole.Position.Z);
        Assert.Equal(45, hole.Direction);
        Assert.Equal(-10, hole.Inclination);
        Assert.Equal(Status.Planned, hole.Status);
        var domainEvent = Assert.Single(
            events,
            domainEvent => domainEvent.EventType == EventType.HoleAdded
        );
        Assert.Equal(EventType.HoleAdded, domainEvent.EventType);
    }

    [Fact]
    public async Task ChargeHoleCommand_WhenHoleDoesNotExist_ThrowsArgumentException()
    {
        var fixture = new CommandsFixture();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Commands.ChargeHoleCommand(Guid.NewGuid(), Guid.NewGuid())
        );

        Assert.Equal("holeId", exception.ParamName);
    }

    [Fact]
    public async Task ChargeHoleCommand_ChargesHoleAndRaisesEvent()
    {
        var fixture = new CommandsFixture();
        var blastId = await fixture.CreateBlastAsync();
        await fixture.Commands.AddHole(blastId, CreateHoleDto());
        var hole = Assert.Single(await fixture.Holes.GetByBlastIdAsync(blastId));

        await fixture.Commands.ChargeHoleCommand(blastId, hole.Id);

        Assert.Equal(Status.Charged, (await fixture.Holes.GetByIdAsync(hole.Id))!.Status);
        var domainEvent = Assert.Single(
            await fixture.Events.GetEventsByAggregateId(hole.Id),
            domainEvent => domainEvent.EventType == EventType.HoleCharged
        );
        Assert.Equal(EventType.HoleCharged, domainEvent.EventType);
    }

    [Theory]
    [InlineData(Status.Charged)]
    [InlineData(Status.Ready)]
    public async Task ChargeHoleCommand_WhenHoleIsAlreadyChargedOrReady_Throws(Status status)
    {
        var fixture = new CommandsFixture();
        var hole = new Hole { Id = Guid.NewGuid(), Status = status };
        await fixture.Holes.AddAsync(hole);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Commands.ChargeHoleCommand(Guid.NewGuid(), hole.Id)
        );
    }

    [Fact]
    public async Task FireBlastCommand_WhenHoleIsNotChargedOrReady_ThrowsInvalidOperationException()
    {
        var fixture = new CommandsFixture();
        var blastId = await fixture.CreateBlastAsync();
        await fixture.Commands.AddHole(blastId, CreateHoleDto());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Commands.FireBlastCommand(blastId)
        );
    }

    [Fact]
    public async Task FireBlastCommand_WhenAllHolesAreCharged_FiresBlastAndRaisesEvent()
    {
        var fixture = new CommandsFixture();
        var blastId = await fixture.CreateBlastAsync();
        await fixture.Commands.AddHole(blastId, CreateHoleDto());
        var hole = Assert.Single(await fixture.Holes.GetByBlastIdAsync(blastId));
        await fixture.Commands.ChargeHoleCommand(blastId, hole.Id);

        await fixture.Commands.FireBlastCommand(blastId);

        var blast = await fixture.Blasts.GetByIdAsync(blastId);
        Assert.NotNull(blast);
        Assert.Equal(Status.Blasted, blast.Status);
        Assert.NotNull(blast.DateBlasted);
        var domainEvent = Assert.Single(
            await fixture.Events.GetEventsByAggregateId(blastId),
            domainEvent => domainEvent.EventType == EventType.BlastFired
        );
        Assert.Equal(EventType.BlastFired, domainEvent.EventType);
    }

    [Fact]
    public async Task FireBlastCommand_WhenBlastDoesNotExist_ThrowsArgumentException()
    {
        var fixture = new CommandsFixture();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Commands.FireBlastCommand(Guid.NewGuid())
        );

        Assert.Equal("blastId", exception.ParamName);
    }

    [Fact]
    public async Task FireBlastCommand_WhenAlreadyBlasted_ThrowsInvalidOperationException()
    {
        var fixture = new CommandsFixture();
        var blastId = await fixture.CreateBlastAsync();
        var blast = await fixture.Blasts.GetByIdAsync(blastId);
        blast!.Status = Status.Blasted;
        await fixture.Blasts.UpdateAsync(blast);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Commands.FireBlastCommand(blastId)
        );
    }

    private static AddHoleDto CreateHoleDto() => new()
    {
        Name = "Hole 1",
        Position = new PositionDto { X = 1, Y = 2, Z = 3 },
        Direction = 45,
        Inclination = -10,
    };

    private sealed class CommandsFixture
    {
        public InMemoryBlastRepository Blasts { get; } = new();
        public InMemoryHolesRepository Holes { get; } = new();
        public InMemoryEventStore Events { get; } = new();
        public Commands.Commands Commands { get; }

        public CommandsFixture()
        {
            Commands = new Commands.Commands(Blasts, Holes, Events);
        }

        public async Task<Guid> CreateBlastAsync() => await Commands.CreateBlastComand(
            new CreateBlastDto { Name = "Blast 1" }
        );
    }
}
