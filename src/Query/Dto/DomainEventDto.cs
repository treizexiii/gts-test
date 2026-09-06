namespace Query.Dto;

public class DomainEventDto
{
    public required Guid Id { get; set; }
    public required string EventType { get; set; }
    public required Guid AggregateId { get; set; }
    public required DateTimeOffset OccurredOn { get; set; }
    public string AggregateType { get; set; } = string.Empty;
}

public enum AggregateTypeDto
{
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    Blast,
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    Hole
}