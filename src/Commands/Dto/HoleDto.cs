namespace Commands.Dto;

public class AddHoleDto
{
    public required string Name { get; set; }
    public PositionDto Position { get; set; } = new();
    public double Direction { get; set; }
    public double Inclination { get; set; }
}

public class PositionDto
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}