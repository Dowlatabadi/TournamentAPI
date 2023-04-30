using MediatR;
using Tournament.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Queries.GetStat
{

[Authorize]
	public record GetContestStatQuery : IRequest<List<QuestionStatDto>>
	{
		public GetContestStatQuery(int contestId)
		{
			ContestId = contestId;

		}
		public int ContestId { get; init; }
	}

	public class GetContestStatQueryHandler : IRequestHandler<GetContestStatQuery,List<QuestionStatDto>>
	{
		private readonly IApplicationDbContext _context;
		public GetContestStatQueryHandler(IApplicationDbContext context, IMapper mapper)
		{
			_context = context;
		}

		public async Task<List<QuestionStatDto>> Handle(GetContestStatQuery request, CancellationToken cancellationToken)
		{
			var ret = _context.Answers.Include(x=>x.Option)
				.Where(x => x.Participation.ContestId == request.ContestId)
				.GroupBy(x => x.Option.QuestionId)
				.Select(a=> new QuestionStatDto{
						QuestionId=a.Key,
						AnswersCount=a.Count(),
						RewadsSpent = a.ToList().Sum(z => z.Participation.Spent),
						OptionsStats =a.GroupBy(z=>z.OptionId).Select(y=>new OptionStatDto {
								OptionId=y.Key,
								RewadsSpent=y.ToList().Sum(z=>z.Participation.Spent),
								Rate=a.Sum(z=>z.Participation.Spent)/ y.ToList().Sum(z => z.Participation.Spent),
								AnswersCount=y.Count(),
								Order=(y.Any())? y.FirstOrDefault().Option.Order-1:0,
								}).ToList()
						});

			return ret.ToList();
		}
	}
}
