namespace Query.Dto;

public class BlastDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? DateBlasted { get; set; } = null;
    public List<HoleDto> Holes { get; set; } = new();
}
