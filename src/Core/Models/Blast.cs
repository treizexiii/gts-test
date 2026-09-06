namespace Core.Models;

public class Blast
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset? DateBlasted { get; set; } = null;
    private Status _status;
    
    public Status Status
    {
        get => _status;
        set => SetStatus(value);
    }

    public void SetStatus(Status status)
    {
        if (status == Status.Charged || status == Status.Ready)
        {
            throw new ArgumentException("Invalid status for a blast.", nameof(status));
        }

        _status = status;
    }
}
