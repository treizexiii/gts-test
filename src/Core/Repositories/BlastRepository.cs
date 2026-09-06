using Core.Models;

namespace Core.Repositories;

public interface IBlastRepository
{
    Task AddAsync(Blast blast);
    Task<Blast?> GetByIdAsync(Guid id);
    Task UpdateAsync(Blast blast);
}

public class InMemoryBlastRepository : IBlastRepository
{
    private readonly List<Blast> _blasts = [];

    public Task AddAsync(Blast blast)
    {
        _blasts.Add(blast);
        return Task.CompletedTask;
    }

    public Task<Blast?> GetByIdAsync(Guid id)
    {
        var blast = _blasts.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(blast);
    }

    public Task UpdateAsync(Blast blast)
    {
        var existingBlast = _blasts.FirstOrDefault(b => b.Id == blast.Id);
        if (existingBlast != null)
        {
            _blasts.Remove(existingBlast);
            _blasts.Add(blast);
        }
        return Task.CompletedTask;
    }
}