using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Exceptions;
using AutoMapper;
using Tournament.Application.Common.Security;

namespace Tournament.Application.Contests.Commands.DrawContest;

[Authorize]
public record DrawContestCommand : IRequest<Unit>
{
	public DrawContestCommand(int id){
		ContestId=id;
	}
	public int ContestId { get; init; }
}

public class DrawContestCommandHandler : IRequestHandler<DrawContestCommand,Unit>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;
	private readonly IDrawService _randomdrawservice;

	public DrawContestCommandHandler(IApplicationDbContext context,IDrawService randomdrawService, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
		_randomdrawservice=randomdrawService;
	}

	public async Task<Unit> Handle(DrawContestCommand request, CancellationToken cancellationToken)
	{
		var contest=await _context.Contests.FindAsync(request.ContestId);
		if (contest==null){
			throw new NotFoundException (nameof(contest),request.ContestId);
		}

		//preapare and count number of participations
		var Questions=await _context.Questions.Where(x=>x.ContestId==request.ContestId).ToListAsync();
		var Participations=await _context.Participations.Include(x=>x.Answers).ThenInclude(x=>x.Option).Where(x=>x.ContestId==request.ContestId).ToListAsync();
		//reset previous draw
		foreach (var p in Participations){
			p.Reward=0;
			p.DrawnRank=0;
		}

		var TotalSpent=Participations.Sum(x=>x.Spent);
		var WinnerParts=Participations.Where(x=>x.Answers.All(y=>y.Option.IsAnswer) && x.Answers.Count()==Questions.Count).Select(x=> (x.Id,x.Spent)).ToList();

		Console.WriteLine($"Found {WinnerParts.Count} Winners for Contest {request.ContestId}");

		//		WinnerParts=WinnerParts.Where(x=>x.Answers.GroupBy(y=> y.Option.QuestionId).All(z=>z.Count()==1)).ToList();

		if (!contest.WeightedDraw){
			WinnerParts=WinnerParts.Select(x=>(x.Item1,1d)).ToList();
		}
		//conduct the draw and determine orders [based on weights]
		var	DrawnList=_randomdrawservice.Draw(WinnerParts,Math.Min(contest.WinnersCapacity,WinnerParts.Count())).ToList();
		var TotalWinnersSpent=WinnerParts.Where(y=> DrawnList.Select(x=>x.item).ToList().Contains(y.Id)).Sum(x=>x.Spent);
		//distribute overall rewards between winners
		foreach (var d in DrawnList){

			var share=0d;
			var part=Participations.Where(x=>x.Id==d.item).First();

			if (contest.WeightedReward){
				share=TotalSpent;
				share=share*(part.Spent/TotalWinnersSpent);
			}
			else{
				share=contest.Reward;
				share/=Math.Min(contest.WinnersCapacity,WinnerParts.Count());
			}
			part.Reward=share;
			part.DrawnRank=d.order;
		}
		contest.Calculated=DateTime.Now;
		await _context.SaveChangesAsync(cancellationToken);
		return Unit.Value;
	}
}

