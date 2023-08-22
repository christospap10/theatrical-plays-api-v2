﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;

namespace Theatrical.Services.Jwt;

public interface ITokenService
{
    JwtDto GenerateToken(User user);
    ClaimsPrincipal? VerifyToken(string token);
}

public class TokenService : ITokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }
    public JwtDto GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtOptions = _config.GetSection("JwtOptions").Get<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        var userIntRole = user.UserAuthorities.FirstOrDefault()?.AuthorityId;

        if (userIntRole == 1)
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        else if (userIntRole == 2)
            claims.Add(new Claim(ClaimTypes.Role, "user"));
        else if (userIntRole == 3)
            claims.Add(new Claim(ClaimTypes.Role, "developer"));
        else if (userIntRole == 4)
            claims.Add(new Claim(ClaimTypes.Role, "claims manager"));
        
        var securityKey = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            SigningCredentials = signingCredentials,
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        var jwtDto = new JwtDto
        {
            access_token = tokenString,
            token_type = "bearer",
            expires_in = (int)TokenLifetime.TotalSeconds
        };

        return jwtDto;
    }
    
    public ClaimsPrincipal? VerifyToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtOptions = _config.GetSection("JwtOptions").Get<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            // Token validation failed
            // You can handle the exception here or return null/throw custom exception based on your needs
            return null;
        }
    }
    

}