using Query.Dto;

namespace Query;

public interface IQuery
{
    Task<BlastDto?> GetBlastQuery(Guid blastId);
    Task<List<DomainEventDto>> GetBlastHistoryQuery(Guid blastId);
}
