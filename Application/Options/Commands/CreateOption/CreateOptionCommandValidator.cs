using FluentValidation;
using Tournament.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Application.Options.Commands.CreateOption;


public class CreateOptionCommandValidator : AbstractValidator<CreateOptionCommand>
{

	private readonly IApplicationDbContext _context;
    public CreateOptionCommandValidator(IApplicationDbContext context)
    {
		_context=context;

		RuleFor(v=>v.QuestionId)
			.NotEmpty().WithMessage("QuestionId is required.")
			.MustAsync(BeValidQuestionId).WithMessage("The Specified QuestionId doesn't Exist.");
    }

	public async Task<bool> BeValidQuestionId(int qId, CancellationToken cancellationToken){
		return await _context.Questions.AnyAsync(x=>x.Id==qId);
	}
}
