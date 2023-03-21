using MediatR;
using Microsoft.EntityFrameworkCore;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Questions.Queries.GetContestQuestions;
using Tournament.Application.Common.Security;
using AutoMapper.QueryableExtensions;
using Tournament.Application.Contests.Queries.GetContests;
using AutoMapper;

namespace Tournament.Application.Options.Queries.GetQuestionOptions;

[Authorize]
public record GetQuestionOptionsQuery : IRequest<List<OptionDto>>
{
	public GetQuestionOptionsQuery(int id)
	{
		QuestionId=id;
	}
	public int QuestionId { get; init; }
}

public class GetQuestionOptionsQueryHandler : IRequestHandler<GetQuestionOptionsQuery, List<OptionDto?>>
{
	private readonly IApplicationDbContext _context;
	private readonly IMapper _mapper;

	// private readonly IMapper _mapper;
	public GetQuestionOptionsQueryHandler(IApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<List<OptionDto>> Handle(GetQuestionOptionsQuery request, CancellationToken cancellationToken)
	{
		return await  _context.Options
			.Where(x => x.QuestionId == request.QuestionId)
            .ProjectTo<OptionDto>(_mapper.ConfigurationProvider)
           .ToListAsync();
	}
}


