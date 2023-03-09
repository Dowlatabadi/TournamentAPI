using Tournament.Application.Common.Interfaces;
using Tournament.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
	private readonly UserManager<ApplicationUser> _userManager;

	public IdentityService(
			UserManager<ApplicationUser> userManager,
			IAuthorizationService authorizationService)
	{
		_userManager = userManager;
	}

	public async Task<bool> AuthenticateUserAsync(string Username,string Password)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == Username);
		if (user!=null && await _userManager.CheckPasswordAsync(user,Password))
		{
			return true;
		}
		return false;
	}

	public async Task<string> GetUserNameAsync(string userId)
	{
		var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

		return user.UserName;
	}

	public async Task<bool> IsInRoleAsync(string userId, string role)
	{
		var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

		return user != null && await _userManager.IsInRoleAsync(user, role);
	}

}
