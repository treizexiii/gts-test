using Commands.Dto;

namespace Commands;

public interface ICommands
{
    Task<Guid> CreateBlastComand(CreateBlastDto blast);
    Task<Guid> AddHole(Guid blastId, AddHoleDto hole);
    Task ChargeHoleCommand(Guid blastId, Guid holeId);
    Task FireBlastCommand(Guid blastId);
}
