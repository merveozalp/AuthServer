using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.ErrorService;
using AuthServer.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetuserByNameAsync(string userName);
        Task<Response<string>> LoginAsync(LoginDto login);
        Task<Response<NoDataDto>> CreateUserRole(string userName);

    }
}
