using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Questions.Queries.GetContestQuestions;

[Authorize]
public record GetContestQuestionsQuery : IRequest<List<QuestionDto>>
{
	public GetContestQuestionsQuery(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}
public class GetContestQuestionsQueryHandler : IRequestHandler<GetContestQuestionsQuery, List<QuestionDto>>
{
	private readonly IApplicationDbContext _context;
	// private readonly IMapper _mapper;
	public GetContestQuestionsQueryHandler(IApplicationDbContext context/*, IMapper mapper*/)
	{
		_context = context;
		// _mapper = mapper;
	}

	public async Task<List<QuestionDto>> Handle(GetContestQuestionsQuery request, CancellationToken cancellationToken)
	{
		return await  _context.Questions
			.Where(x => x.ContestId == request.ContestId).
			Select(x=> new QuestionDto
					{
					ContestId = x.ContestId,
					Id = x.Id,
					Title = x.Title,
					Resolved=x.Resolved, 
					Options=x.Options.Select(y=>
							new OptionDto {
							Id=y.Id,
							Title=y.Title,
							Text=y.Text,
							IsAnswer=y.IsAnswer
							}
							).ToList()
					}).ToListAsync();
	}
}

