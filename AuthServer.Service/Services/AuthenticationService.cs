using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.ErrorService;
using AuthServer.Core.Repository;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _client;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefrehToken;

        public AuthenticationService(IOptions<List<Client>> client, ITokenService tokenService, UserManager<UserApp> userManager, IGenericRepository<UserRefreshToken> repositoyry, IUnitOfWork unitOfWork)
        {
            _client = client.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _userRefrehToken = repositoyry;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentException(nameof(loginDto));
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail("Girilen veya Şifre Hatalı",400,true);
            if(!await _userManager.CheckPasswordAsync(user,loginDto.Password))
            {
                return Response<TokenDto>.Fail("Girilen veya Şifre Hatalı", 400, true);
            }
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefrehToken.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            if(userRefreshToken==null)
            {
                await _userRefrehToken.AddAsync(new UserRefreshToken { UserId = user.Id,RefreshTokenCode=token.RefreshToken,Expiration=token.RefreshTokenExpiration});
            }
            else  // refreshTOken varsa güncelleme yapmasını istedik.
            {
                userRefreshToken.RefreshTokenCode = token.RefreshToken;
                userRefreshToken.Expiration= token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token,200);

            

            
        }  // Kullanıcı için token 

        //public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        //{
        //    var client = _client.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
        //    if (client == null)
        //    {
        //        return Response<ClientTokenDto>.Fail("Id ve Secret bulunamadı", 404, true);
        //    }

        //    var token = _tokenService.CreateTokenByClient(client);
        //    return Response<ClientTokenDto>.Success(token, 200);
        //} // Kullanıcı olamayan Clientlar için token

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var refToken = await _userRefrehToken.Where(x => x.RefreshTokenCode == refreshToken).SingleOrDefaultAsync();
            if(refToken==null)
            {
                return Response<TokenDto>.Fail("Refresh Token bulunamadı", 404, true);

            }
            var user = await _userManager.FindByIdAsync(refToken.UserId);
            if(user == null) { return Response<TokenDto>.Fail("User Id bulunamadı", 404, true); }
            var tokenDto = _tokenService.CreateToken(user);
            refToken.RefreshTokenCode = tokenDto.RefreshToken;
            refToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);


        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var refToken = await _userRefrehToken.Where(x => x.RefreshTokenCode == refreshToken).SingleOrDefaultAsync();
            if(refToken==null) { return Response<NoDataDto>.Fail("Refresh token Bulunamadı",404,true); }
            _userRefrehToken.Remove(refToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}
