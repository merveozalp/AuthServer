using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.ErrorService;
using AuthServer.Core.Repository;
using AuthServer.Core.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(UserManager<UserApp> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, SignInManager<UserApp> signInManager, ITokenService tokenService, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }



        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
            };

            var result = await _userManager.CreateAsync(user,createUserDto.Password);
            if(!result.Succeeded) 
            {
              var errors = result.Errors.Select(x=>x.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }
            return Response<UserAppDto>.Success(_mapper.Map<UserAppDto>(user), 200);
         }

        public async Task<Response<NoDataDto>> CreateUserRole(string userName)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new() { Name = "user" });
                await _roleManager.CreateAsync(new() { Name = "Admin" });
            }
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, "Admin");
            await _userManager.AddToRoleAsync(user, "manager");

            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<UserAppDto>> GetuserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user==null)
            {
                return Response<UserAppDto>.Fail("Böyle bir kullanıcı nulunamadı", 404, true);
            }
            return Response<UserAppDto>.Success(_mapper.Map < UserAppDto>(user),200);
        }

        public async Task<Response<string>> LoginAsync(LoginDto login)
        {
            var signUser = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
            if (signUser is null) return Response<string>.Fail("Hatalı Giriş", 404, true);

            var userByEmnail = await _userManager.FindByEmailAsync(login.Email);
            var userRole = await _userManager.GetRolesAsync(userByEmnail);

            //var token = _tokenService.CreateToken(login).ToString();
            return Response<string>.Success(200);
        }
    }
}
