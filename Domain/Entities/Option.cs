namespace Tournament.Domain.Entities;

public class Option : BaseAuditableEntity
{
	public int QuestionId { get; set; }
	public int Order { get; set; }
	public Question Question { get; set; }
	public string? Title { get; set; }
	public string? Text { get; set; }
	public bool IsAnswer { 
		get => _isAnswer; 
		set
		{
			if (value==true){
				AddDomainEvent(new OptionisCorrectEvent(this));
			}
			_isAnswer=value;
		}
	}
	private bool _isAnswer;
    public IList<Answer> Answer { get; private set; } = new List<Answer>(); 
}

