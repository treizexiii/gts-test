using Core.Events;
using Core.Repositories;
using Query.Dto;

namespace Query;

public class Query : IQuery
{
    private readonly IBlastRepository _blastRepository;
    private readonly IHolesRepository _holesRepository;
    private readonly IEventStore _eventStore;

    public Query(
        IBlastRepository blastRepository,
        IHolesRepository holesRepository,
        IEventStore eventStore
    )
    {
        _blastRepository = blastRepository;
        _holesRepository = holesRepository;
        _eventStore = eventStore;
    }

    public async Task<BlastDto?> GetBlastQuery(Guid blastId)
    {
        var blast = await _blastRepository.GetByIdAsync(blastId);
        if (blast is null)
        {
            throw new ArgumentException($"Blast with ID {blastId} not found.", nameof(blastId));
        }

        var holes = await _holesRepository.GetByBlastIdAsync(blastId);

        var dto = DtoBuilder.BuildBlastDto(blast, [.. holes]);

        return dto;
    }

    public async Task<List<DomainEventDto>> GetBlastHistoryQuery(Guid blastId)
    {
        var blast = await _blastRepository.GetByIdAsync(blastId);
        if (blast is null)
        {
            throw new ArgumentException($"Blast with ID {blastId} not found.", nameof(blastId));
        }

        var events = await _eventStore.GetEventsByAggregateId(blastId);

        foreach (var hole in await _holesRepository.GetByBlastIdAsync(blastId))
        {
            var holeEvents = await _eventStore.GetEventsByAggregateId(hole.Id);
            events = events.Concat(holeEvents);
        }

        var eventDtos = events
            .OrderBy(e => e.OccurredOn)
            .Select(e => DtoBuilder.BuildDomainEventDto(e))
            .ToList();

        return eventDtos;
    }
}
