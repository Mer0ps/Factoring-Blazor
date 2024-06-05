namespace Domain.Models;
public class EventNotification
{
    public IEnumerable<string> GroupsName { get; set; }
    public string EventName { get; set; }
    public NotificationType Type { get; set; }
}

public enum NotificationType
{
    All,
    Group,
    Client
}
