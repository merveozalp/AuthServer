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
using System.Threading.Tasks;

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
        private async Task<IEnumerable<Claim>> GetClaim(UserApp userApp,List<String> audinces)
        {

            var userRoles = await _userManager.GetRolesAsync(userApp);
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            userList.AddRange(audinces.Select(x=> new Claim(JwtRegisteredClaimNames.Aud,x)));
            userList.AddRange(userRoles.Select(x=>new Claim(ClaimTypes.Role,x)));
            return userList;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration); // Token Süresi
            var refrshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration); // Token Süresi
           var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey); // Token imzalyacak olan Key


            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);// Verilen algoritma ile imza oluşturulur.


            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims:GetClaim(userApp, _tokenOption.Audience).Result,
               signingCredentials: signingCredentials
               );

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

        //private IEnumerable<Claim> GetClaimsByClient(Client client)   // Üyelik sistemi gerektirmeyen durumlarda geçerli Token
        //{
        //    var claims = new List<Claim>();
        //    claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        //    new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());
        //    return claims;
        //}

        //public ClientTokenDto CreateTokenByClient(Client client)
        //{
        //    var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration); // Token Süresi
        //    var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey); // Token imzalyacak olan Key
        //    SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature); // Verilen algoritma ile imza oluşturulur.
        //    JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
        //        issuer: _tokenOption.Issuer,
        //        expires: accessTokenExpiration,
        //        notBefore: DateTime.Now,
        //        claims: GetClaimsByClient(client),
        //        signingCredentials: signingCredentials);

        //    var handler = new JwtSecurityTokenHandler();
        //    var token = handler.WriteToken(jwtSecurityToken);
        //    var tokenDto = new ClientTokenDto
        //    {
        //        AccessToken = token,
        //        AccessTokenExpiration = accessTokenExpiration,

        //    };
        //    return tokenDto;
        //}
    }
}
