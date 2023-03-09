namespace Tournament.Domain.Entities;

public class Question : BaseAuditableEntity
{
    public int ContestId { get; set; }
    public Contest Contest { get; set; }
    public string? Title { get; set; }
    public bool Resolved {get; set;}=false; 
    public IList<Option> Options { get; init; } = new List<Option>(); 
}


