using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUserService userService, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            var result = await _userService.CreateUserAsync(createUserDto);
            if(result.StatusCode==200)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var result = await _userService.GetuserByNameAsync(HttpContext.User.Identity.Name);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRoles(string name)
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = name });
            return Ok(name);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var loginUser = await  _userService.LoginAsync(login);
            if(loginUser==null) return BadRequest();
            return Ok(loginUser);

        }

        
    }
}
