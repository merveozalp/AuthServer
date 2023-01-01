using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;

        private readonly CustomTokenOptions _tokenOption;

       

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> tokenOption)
        {
            _userManager = userManager;
            _tokenOption = tokenOption.Value;
        }
        private string CreateRefreshToken()
        {
            var number = new Byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }


        // Payload olmasını istediğim tüm dataları Claim olarak ekliyorum.
        private IEnumerable<Claim> GetClaim(UserApp userApp,List<String> audinces)
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName)
            };

            userList.AddRange(audinces.Select(x=> new Claim(JwtRegisteredClaimNames.Aud,x)));
            return userList;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration); // Token Süresi
            var refrshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration); // Token Süresi
            var securityKey =SignService.GetSecurityKey(_tokenOption.SecurityKey); // Token imzalyacak olan Key
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature); // Verilen algoritma ile imza oluşturulur.
            JwtSecurityToken jwtSecurityToken =new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore:DateTime.Now,
                claims:GetClaim(userApp,_tokenOption.Audince),
                signingCredentials:signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refrshTokenExpiration
            };
            return tokenDto;

        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
