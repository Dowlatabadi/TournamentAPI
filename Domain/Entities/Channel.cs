namespace Tournament.Domain.Entities;

public class Channel : BaseAuditableEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IList<Contest> Contests { get; private set; } = new List<Contest>(); 
}

