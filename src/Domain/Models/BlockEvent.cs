namespace Domain.Models;
public class BlockEvent
{
    public IEnumerable<Event> Events { get; set; }
    public string Hash { get; set; }
    public string ShardId { get; set; }
    public int Timestamp { get; set; }
}
public class Event
{
    public string Address { get; set; }
    public string? Data { get; set; }
    public string Identifier { get; set; }
    public IEnumerable<string> Topics { get; set; }
    public string TxHash { get; set; }
}
