
namespace Tournament.Domain.Entities;

public class Answer : BaseAuditableEntity
{
    public int  OptionId { get; set; }
    public Option  Option { get; set; }
    public int  ParticipationId { get; set; }
    public Participation  Participation { get; set; }
}

