using Tournament.Domain.Entities;
using Tournament.Application.Common.Mappings;

namespace Tournament.Application.Questions.Queries.GetContestQuestions;

public class QuestionDto:IMapFrom<Question>
{
	public int Id { get; set; }
	public int  ContestId { get; set; }
	public string?  Title { get; set; }
	public bool  Resolved { get; set; }
	public IList<OptionDto>  Options { get; set; }
}	
public class OptionDto:IMapFrom<Option>
{
	public int Id { get; set; }
	public string?  Title { get; set; }
	public string?  Text { get; set; }
	public bool  IsAnswer { get; set; }
}
