namespace Tournament.Domain.Entities;

public class Contest : BaseAuditableEntity
{
	public int ChannelId {get; set; }
	public Channel Channel {get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime?  Start { get; set; }
    public DateTime?  Finish { get; set; }
    public bool IsActive {get; set;}=false; 
    public bool Resolved {get; set;}=false; 
    public bool WeightedDraw {get; set;}=false;
    public bool WeightedReward {get; set;}=false;
    public double Reward {get; set;} 
    public int WinnersCapacity {get; set;} 
    public int ParticipationCapacity { get; set;} 
    public IList<Question> Questions { get; init; } = new List<Question>(); 


    public IList<Participation> Participations { get; init; } = new List<Participation>(); 
    public DateTime? Calculated { get; set; }
}

