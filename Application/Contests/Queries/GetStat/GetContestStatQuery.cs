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

    public class GetContestStatQueryHandler : IRequestHandler<GetContestStatQuery, List<QuestionStatDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetContestStatQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<List<QuestionStatDto>> Handle(GetContestStatQuery request, CancellationToken cancellationToken)
        {
            var res = new List<QuestionStatDto>();
            var ret = _context.Answers.Include(x => x.Option)
                .Where(x => x.Participation.ContestId == request.ContestId)
                .GroupBy(x => x.Option.QuestionId)
                .Select(a => new QuestionStatDto
                {
                    QuestionId = a.Key,
                    AnswersCount = a.Count(),
                    RewadsSpent = a.ToList().Sum(z => z.Participation.Spent),
                    OptionsStats = a.GroupBy(z => z.OptionId).Select(y => new OptionStatDto
                    {
                        OptionId = y.Key,
                        RewadsSpent = y.ToList().Sum(z => z.Participation.Spent),
                        Rate = a.Sum(z => z.Participation.Spent) / y.ToList().Sum(z => z.Participation.Spent),
                        AnswersCount = y.Count(),
                        Order = (y.Any()) ? y.FirstOrDefault().Option.Order - 1 : 0,
                    }).ToList()
                }).AsNoTracking();

            //get all options
            var all_Questions_Options = _context.Questions.Include(x => x.Options)
                .Where(x => x.ContestId == request.ContestId)
                .ToDictionary(x => x.Id, x => x.Options.Select(x => new { x.Id, x.Order }));

            //fill in the missing options with 0 stats
            foreach (var keyval in all_Questions_Options)
            {
                var q = ret.Where(x => x.QuestionId == keyval.Key).FirstOrDefault();
                var stats = new List<OptionStatDto>();
                foreach (var option in keyval.Value)
                {
                    if (!q.OptionsStats.Select(x => x.OptionId).Contains(option.Id))
                    {
                        stats.Add(
                            new OptionStatDto
                            {
                                OptionId = option.Id,
                                AnswersCount = 0,
                                RewadsSpent = 0,
                                Rate = 0,
                                Order = option.Order-1,
                            });
                    }
                    else
                    {
                        var stat = q.OptionsStats.FirstOrDefault(x => x.OptionId == option.Id);
                        stats.Add(stat);
                    }


                }
                res.Add(new QuestionStatDto
                {
                    AnswersCount = q.AnswersCount,
                    OptionsStats = stats,
                    QuestionId = q.QuestionId,
                    RewadsSpent = q.RewadsSpent,
                });
            }




            return res.ToList();
        }
    }
}
