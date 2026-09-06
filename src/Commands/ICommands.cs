using Commands.Dto;

namespace Commands;

public interface ICommands
{
    Task<Guid> CreateBlastComand(CreateBlastDto blast);
    Task AddHole(Guid blastId, AddHoleDto hole);
    Task ChargeHoleCommand(Guid blastId, Guid holeId);
    Task FireBlastCommand(Guid blastId);
}
