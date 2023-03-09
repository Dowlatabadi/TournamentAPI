
namespace Tournament.Application.Common.Interfaces;

public interface IIdentityService
{
	Task<bool> AuthenticateUserAsync(string Username,string Password);
	Task<string> GetUserNameAsync(string userId);
	Task<bool> IsInRoleAsync(string userId, string role);
}
