using Tournament.Application.Common.Interfaces;
using MediatR;

namespace Tournament.Application.Auth.Queries.Authenticate;

public record AuthenticateQuery : IRequest<bool>{
	public string Username { get; set; }
	public string Password { get; set; }
}
public class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery,bool>
{
	private readonly IIdentityService _identityservice;
	public AuthenticateQueryHandler(IIdentityService identityservice)
	{
		_identityservice=identityservice;
	}
	public async Task<bool> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
	{
		if ( await _identityservice.AuthenticateUserAsync(request.Username,request.Password)){
			return true;
		}
			throw new UnauthorizedAccessException();
	}
}
