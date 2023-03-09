namespace Tournament.Domain.Entities;

public class Participation : BaseAuditableEntity
{
	public string AccountId { get; set; }
	public int  ContestId { get; set; }
	public Contest  Contest { get; set; }
	public double  Spent { get; set; }
	public int  DrawnRank { get; set; }=0;
	public double  Reward { get; set; }=0;
	public IList<Answer> Answers { get; set; } = new List<Answer>(); 
}

