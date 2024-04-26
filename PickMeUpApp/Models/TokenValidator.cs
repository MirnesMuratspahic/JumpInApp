using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenValidator
{
    private readonly string _securityKey;

    public TokenValidator(string securityKey)
    {
        _securityKey = securityKey;
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };

        SecurityToken validatedToken;
        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return true; // Token je valjan
        }
        catch
        {
            return false; // Token nije valjan
        }

    }
    public JwtSecurityToken DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ReadJwtToken(token);
    }
}
