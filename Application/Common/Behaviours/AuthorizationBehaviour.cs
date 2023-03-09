using System.Reflection;
using Tournament.Application.Common.Exceptions;
using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Security;
using MediatR;

namespace Tournament.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly ICurrentUserService _currentUserService;
	private readonly IIdentityService _identityService;

	public AuthorizationBehaviour(
			ICurrentUserService currentUserService,
			IIdentityService identityService)
	{
		_currentUserService = currentUserService;
		_identityService = identityService;
	}
	public async Task<TResponse> Handle(TRequest request,  RequestHandlerDelegate<TResponse> next,CancellationToken cancellationToken)
	{
		var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

		if (authorizeAttributes.Any())
		{
			// Must be authenticated user
			if (_currentUserService.UserName == null)
			{
				throw new UnauthorizedAccessException();
			}

			// Role-based authorization
			var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

			if (authorizeAttributesWithRoles.Any())
			{
				var authorized = false;

				foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
				{
					foreach (var role in roles)
					{
						var isInRole = await _identityService.IsInRoleAsync(_currentUserService.UserName, role.Trim());
						if (isInRole)
						{
							authorized = true;
							break;
						}
					}
				}

				// Must be a member of at least one role in roles
				if (!authorized)
				{
					throw new ForbiddenAccessException();
				}
			}

		}

		// User is authorized / authorization not required
		return await next();
	}
}
