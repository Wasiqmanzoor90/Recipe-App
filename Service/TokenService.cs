using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using WebApplication1.Interface;

namespace WebApplication1.Service
{
    public class TokenService : ITokenService
    {
        private readonly string _secretkey;
        public TokenService(IConfiguration configuration)
        {
            _secretkey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Server error");
        }
        public string CreateToken(ObjectId id, string Email)
        {
            try
            {


                var tokenhandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretkey);
                var tokendescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([
                       new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, Email)
                   ]),
                    Expires = DateTime.UtcNow.AddHours(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

                };
                var token = tokenhandler.CreateToken(tokendescriptor);
                return tokenhandler.WriteToken(token);
            }
            catch (Exception ex)
            {

                throw new Exception("Server error: " + ex.Message);
            }
        }

        public ObjectId VerifyToken(string token)
        {
            try
            {
                var tokenhandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretkey);
                var validatetoken = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                var principal = tokenhandler.ValidateToken(token, validatetoken, out var validatedToken);
                var userIdclaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdclaim != null)
                {
                    return new ObjectId(userIdclaim.Value);
                }
                else
                {
                    throw new Exception("User ID not found in token.");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Token validation failed: " + ex.Message);
            }
        }
    }
}