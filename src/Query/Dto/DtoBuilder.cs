using Core.Events;
using Core.Models;

namespace Query.Dto;

public class DtoBuilder
{
    public static BlastDto BuildBlastDto(Blast blast, List<Hole>? holes = null)
    {
        var dto = new BlastDto
        {
            Id = blast.Id,
            Name = blast.Name,
            Status = blast.Status.ToString(),
            DateBlasted = blast.DateBlasted,
        };

        if (holes != null)
        {
            dto.Holes =
            [
                .. holes.Select(h => new HoleDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Position = new PositionDto
                    {
                        X = h.Position.X,
                        Y = h.Position.Y,
                        Z = h.Position.Z,
                    },
                    Direction = h.Direction,
                    Inclination = h.Inclination,
                    Status = h.Status.ToString(),
                }),
            ];
        }

        return dto;
    }

    public static DomainEventDto BuildDomainEventDto(IDomainEvent domainEvent)
    {
        return new DomainEventDto
        {
            Id = domainEvent.EventId,
            EventType = domainEvent.EventType.ToString(),
            AggregateId = domainEvent.AggregateId,
            AggregateType = domainEvent.AggregateType switch
            {
                nameof(Blast) => AggregateTypeDto.Blast.ToString(),
                nameof(Hole) => AggregateTypeDto.Hole.ToString(),
                _ => throw new ArgumentException($"Unknown aggregate type: {domainEvent.AggregateType}")
            },
            OccurredOn = domainEvent.OccurredOn,
        };
    }
}
