using Commands.Dto;
using Core.Events;
using Core.Models;
using Core.Repositories;

namespace Commands;

public class Commands : ICommands
{
    private readonly IBlastRepository _blastRepository;
    private readonly IHolesRepository _holesRepository;
    private readonly IEventStore _eventStore;

    public Commands(
        IBlastRepository blastRepository,
        IHolesRepository holesRepository,
        IEventStore eventStore
    )
    {
        _blastRepository = blastRepository;
        _holesRepository = holesRepository;
        _eventStore = eventStore;
    }

    public async Task<Guid> CreateBlastComand(CreateBlastDto blast)
    {
        var newBlast = new Blast
        {
            Id = Guid.NewGuid(),
            Name = blast.Name,
            Status = Status.Planned,
        };

        await _blastRepository.AddAsync(newBlast);

        await _eventStore.RaiseEventAsync(EventType.BlastCreated, newBlast.Id);
        return newBlast.Id;
    }

    public async Task AddHole(Guid blastId, AddHoleDto hole)
    {
        var blast = await _blastRepository.GetByIdAsync(blastId);

        if (blast == null)
        {
            throw new ArgumentException($"Blast with ID {blastId} not found.", nameof(blastId));
        }

        var newHole = new Hole
        {
            Id = Guid.NewGuid(),
            BlastId = blastId,
            Name = hole.Name,
            Position = new Position
            {
                X = hole.Position.X,
                Y = hole.Position.Y,
                Z = hole.Position.Z,
            },
            Direction = hole.Direction,
            Inclination = hole.Inclination,
            Status = Status.Planned,
        };

        await _holesRepository.AddAsync(newHole);

        await _eventStore.RaiseEventAsync(EventType.HoleAdded, newHole.Id);
    }

    public async Task ChargeHoleCommand(Guid blastId, Guid holeId)
    {
        var hole = await _holesRepository.GetByIdAsync(holeId);
        if (hole == null)
        {
            throw new ArgumentException($"Hole with ID {holeId} not found.", nameof(holeId));
        }

        if (hole.Status == Status.Charged || hole.Status == Status.Ready)
        {
            throw new InvalidOperationException(
                $"Hole with ID {holeId} cannot be charged because its status is {hole.Status}."
            );
        }

        hole.Status = Status.Charged;

        await _holesRepository.UpdateAsync(hole);

        await _eventStore.RaiseEventAsync(EventType.HoleCharged, hole.Id);
    }

    public async Task FireBlastCommand(Guid blastId)
    {
        var blast = await _blastRepository.GetByIdAsync(blastId);

        if (blast == null)
        {
            throw new ArgumentException($"Blast with ID {blastId} not found.", nameof(blastId));
        }

        var holes = await _holesRepository.GetByBlastIdAsync(blastId);

        foreach (var hole in holes)
        {
            if (hole.Status == Status.Ready)
            {
                continue;
            }
            if (hole.Status == Status.Charged)
            {
                continue;
            }
            throw new InvalidOperationException(
                $"Cannot fire blast because hole with ID {hole.Id} is not ready or charged."
            );
        }

        if (blast.Status == Status.Blasted)
        {
            throw new InvalidOperationException(
                $"Blast with ID {blastId} has already been fired."
            );
        }

        blast.Status = Status.Blasted;
        blast.DateBlasted = DateTimeOffset.UtcNow;
        await _blastRepository.UpdateAsync(blast);

        await _eventStore.RaiseEventAsync(EventType.BlastFired, blast.Id);
    }
}
