using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Questions.Commands.CreateQuestion;


public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{

	private readonly IApplicationDbContext _context;
    public CreateQuestionCommandValidator(IApplicationDbContext context)
    {
		_context=context;

		RuleFor(v=>v.ContestId)
			.NotEmpty().WithMessage("ContestId is required.")
			.MustAsync(BeValidQuestionId).WithMessage("The Specified ContestId doesn't Exist.");
    }

	public async Task<bool> BeValidQuestionId(int cId, CancellationToken cancellationToken){
		return await _context.Contests.AnyAsync(x=>x.Id==cId);
	}
}
