namespace Tournament.WebAPI.Authorization;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public interface IJwtUtils
{
    public string GenerateToken(string username);
    public string? ValidateToken(string token);
}
public class jwtOptions {
    public string phrase { get; set; }
    public int minutes { get; set; }
}
public class JwtUtils : IJwtUtils
{
	private readonly jwtOptions _options;

    public JwtUtils(IConfiguration configuration,IOptions<jwtOptions> options)
    {
        _options=options.Value;
    }

    public string GenerateToken(string username)
    {
        // generate token that is valid for minutes
        var tokenHandler = new JwtSecurityTokenHandler();
		var phrase=_options.phrase;
		var minutes=_options.minutes;
        var key = Encoding.ASCII.GetBytes(phrase);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", username) }),
            Expires = DateTime.Now.AddMinutes(minutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        if (token == null) 
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
		var phrase=_options.phrase;
        var key = Encoding.ASCII.GetBytes(phrase);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.First(x => x.Type == "username").Value;

            // return username  from JWT token 
            return username;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}
