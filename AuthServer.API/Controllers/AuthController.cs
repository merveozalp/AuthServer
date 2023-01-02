using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result = await _authenticationService.CreateTokenAsync(loginDto);
            if (result.StatusCode == 200)
            {
                return Ok(result.Data);
            }
            return BadRequest();
        }
        //[HttpPost]
        //public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        //{
        //    var result =  _authenticationService.CreateTokenByClient(clientLoginDto);
        //    if (result.StatusCode == 200)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest();
        //}

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.RevokeRefreshToken(refreshTokenDto.RefreshToken);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.RefreshToken);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
