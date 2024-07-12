namespace Defast.Bot.Domain.Entities.Identity;

public class Session
{
    public string? SessionId { get; set; }
    
    public string? Version { get; set; }
    
    public int SessionTimeout { get; set; }
}
