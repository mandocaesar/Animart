﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Animart.Portal.Authorization;
using Animart.Portal.Sessions.Dto;
using Animart.Portal.User.Dto;
using Animart.Portal.Users;
using Animart.Portal.Users.Dto;
using Microsoft.AspNet.Identity;

namespace Animart.Portal.Users
{
    public class UserAppService: PortalAppServiceBase, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly UserStore _userStore;


        public UserAppService(UserManager userManager, RoleManager roleManager, IRepository<Users.User, long> userRepository, UserStore userStore)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userStore = userStore;
        }

        public async Task CreateUser(RegisterUserInput user)
        {
            var _user = new User
            {
                TenantId = 1,
                UserName = user.UserName,
                Name = user.FirstName,
                Surname = user.LastName,
                EmailAddress = user.Email,
                IsEmailConfirmed = true,
                Password = new PasswordHasher().HashPassword("ZXasqw12"),
                CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                CreatorUserId = AbpSession.GetUserId()

            };
        }

        public async Task UpdateUser(UserDto user)
        {
            var _user = _userRepository.Get(user.Id);
            var roleId = _userRepository.Get(user.Id).Roles.First().RoleId;

            _user.IsActive = user.IsActive;
            _user.Name = user.FirstName;
            _user.Surname = user.LastName;
            _user.EmailAddress = user.Email;
            user.Role = _roleManager.FindById(roleId).DisplayName;

            _userRepository.Update(_user);
        }

        public async Task UpdateUserRole(int userId, string role)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDto> GetUsers()
        {
            var users =  _userRepository.GetAll().Select(e => new UserDto
            {
                Id = e.Id,
                FirstName = e.Name,
                LastName = e.Surname,
                IsActive = e.IsActive,
                Email = e.EmailAddress,
                LastLoginTime = e.LastLoginTime.Value
            }).ToList();

            foreach (var user in users)
            {
                var roleId = _userRepository.Get(user.Id).Roles.First().RoleId;
                user.Role = _roleManager.FindById(roleId).DisplayName;
            }
            return users;
        }

        public ListResultOutput<UserDto> GetUsersList()
        {
            var b = _userManager.Users;
            var a = b.ToList().MapTo<List<UserDto>>();
            return new ListResultOutput<UserDto>
            {
                Items = a
            };
        }
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                User = (await GetCurrentUserAsync()).MapTo<UserLoginInfoDto>()
            };

            if (AbpSession.TenantId.HasValue)
            {
                // output.Tenant = (await GetCurrentTenantAsync()).MapTo<TenantLoginInfoDto>();
            }

            return output;
        }
    }
}
