using Core.Models;

namespace Core.Repositories;

public interface IHolesRepository
{
    Task AddAsync(Hole hole);
    Task<Hole?> GetByIdAsync(Guid id);
    Task<IEnumerable<Hole>> GetByBlastIdAsync(Guid blastId);
    Task UpdateAsync(Hole hole);
}

public class InMemoryHolesRepository : IHolesRepository
{
    private readonly List<Hole> _holes = [];

    public Task AddAsync(Hole hole)
    {
        _holes.Add(hole);
        return Task.CompletedTask;
    }

    public Task<Hole?> GetByIdAsync(Guid id)
    {
        var hole = _holes.FirstOrDefault(h => h.Id == id);
        return Task.FromResult(hole);
    }

    public Task<IEnumerable<Hole>> GetByBlastIdAsync(Guid blastId)
    {
        var holes = _holes.Where(h => h.BlastId == blastId);
        return Task.FromResult(holes);
    }

    public Task UpdateAsync(Hole hole)
    {
        var existingHole = _holes.FirstOrDefault(h => h.Id == hole.Id);
        if (existingHole is not null)
        {
            _holes.Remove(existingHole);
            _holes.Add(hole);
        }
        return Task.CompletedTask;
    }
}