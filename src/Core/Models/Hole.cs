namespace Core.Models;

public class Hole
{
    public Guid Id { get; set; }
    public Guid BlastId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Position Position { get; set; } = new Position();
    private double _direction;
    public double Direction
    {
        get => _direction;
        set => SetDirection(value);
    }
    private double _inclination;
    public double Inclination
    {
        get => _inclination;
        set => SetInclination(value);
    }
    private Status _status;
    public Status Status
    {
        get => _status;
        set => SetStatus(value);
    }

    public void SetDirection(double direction)
    {
        if (direction < 0 || direction > 360)
        {
            throw new ArgumentOutOfRangeException(
                nameof(direction),
                "Direction must be between 0 and 360 degrees."
            );
        }

        _direction = direction;
    }

    public void SetInclination(double inclination)
    {
        if (inclination < -90 || inclination > 90)
        {
            throw new ArgumentOutOfRangeException(
                nameof(inclination),
                "Inclination must be between -90 and 90 degrees."
            );
        }

        _inclination = inclination;
    }

    public void SetStatus(Status status)
    {
        if (status == Status.Loaded || status == Status.Blasted)
        {
            throw new ArgumentException("Invalid status for a hole.", nameof(status));
        }

        _status = status;
    }
}
