using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Animart.Portal.Authorization;
using Animart.Portal.Extension;
using Animart.Portal.Sessions.Dto;
using Animart.Portal.User.Dto;
using Animart.Portal.Users;
using Animart.Portal.Users.Dto;
using Microsoft.AspNet.Identity;
using AutoMapper;

namespace Animart.Portal.Users
{
    public class UserAppService: PortalAppServiceBase, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly UserStore _userStore;


        public UserAppService(UserManager userManager, RoleManager roleManager, 
            IRepository<Users.User, long> userRepository, UserStore userStore)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userStore = userStore;
        }

        public async Task CreateUser(RegisterUserInput user)
        {
            var role =_roleManager.FindByName(user.Role);
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
            _user.Roles = new List<UserRole> {new UserRole() {RoleId = role.Id}};
            _userManager.Create(_user);

            GmailExtension gmail = new GmailExtension(GmailExtension.ANIMART_EMAILADDRESS, GmailExtension.ANIMART_PASSWORD);
            gmail.SendMessage(
                "Welcome Aboard !", 
                string.Format("<p> Welcome,</p><br/><br/><br/> <p> Your Animart registration has now been approved and your account is ready to use.<br/><br/>You may now log on to the system on http://shop.animart.co.id </p>" +
                "<br/><br/><p> Your user - id is: {0} and your password: ZXasqw12 </p><br/><br/><p> If you have any queries, please contact the Animart on + 62(22) 612 - 6824 </p>" +
                "<br/><br/><br/><p> Sincerely, </p><br/><br/><br/><p> PT.Animart Hobi Kreatif <br/>Tel: (+62) 22 - 612 - 6824 </p>", _user.UserName),
                _user.EmailAddress,false,null,null);

        }

        public async Task UpdateUser(UserDto user)
        {
            try
            {
                var _user = _userRepository.Get(user.Id);
                if (_userRepository.Get(user.Id).Roles.Count > 0)
                {
                    var roleId = _userRepository.Get(user.Id).Roles.First().RoleId;
                    var currentRole = _roleManager.FindById(roleId).DisplayName;
                    if (currentRole != user.Role)
                    {
                        UserManager.RemoveFromRole(_user.Id, currentRole);
                        UserManager.AddToRole(_user.Id, user.Role);
                    }
                }
               
                _user.IsActive = user.IsActive;
                _user.Name = user.FirstName;
                _user.Address = user.Address;
                _user.Surname = user.LastName;
                _user.EmailAddress = user.Email;
                _user.PhoneNumber = user.PhoneNumber;

                _userRepository.Update(_user);
              
            }
            catch (Exception ex)
            {       
                throw ex;
            }
          
        }


        public bool UpdateUserProfile(UserProfileDto user)
        {
            try
            {
                var _user = _userRepository.Get(user.Id);
                //var roleId = _userRepository.Get(user.Id).Roles.First().RoleId;
                //var currentRole = _roleManager.FindById(roleId).DisplayName;
                //var checkPassword = new PasswordHasher().VerifyHashedPassword(_user.Password, user.Password);
                //if (checkPassword == PasswordVerificationResult.Success)
                //{
                    //_user.IsActive = user.IsActive;
                    _user.Name = user.FirstName;
                    _user.Address = user.Address;
                    _user.PhoneNumber = user.PhoneNumber;
                    _user.Surname = user.LastName;
                    //_user.Password = new PasswordHasher().HashPassword(user.NewPassword);
                    _userRepository.Update(_user);
                    return true;
                //}
                //return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdatePassword(UserProfileDto user)
        {
            try
            {
                var _user = _userRepository.Get(user.Id);
                var roleId = _userRepository.Get(user.Id).Roles.First().RoleId;
                var currentRole = _roleManager.FindById(roleId).DisplayName;
                var checkPassword = new PasswordHasher().VerifyHashedPassword(_user.Password, user.Password);
                if (checkPassword == PasswordVerificationResult.Success)
                {
                    //_user.IsActive = user.IsActive;
                    //_user.Name = user.FirstName;
                    //_user.Address = user.Address;
                    //_user.Surname = user.LastName;
                    _user.Password = new PasswordHasher().HashPassword(user.NewPassword);
                    _userRepository.Update(_user);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task Delete(UserDto user)
        {
            var _user = _userRepository.Get(user.Id);
            UserManager.Delete(_user);
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
                Address = e.Address,
                PhoneNumber = e.PhoneNumber,
                IsActive = e.IsActive,
                Email = e.EmailAddress,
                UserName = e.UserName,
                LastLoginTime = e.LastLoginTime
            }).ToList();

            foreach (var user in users)
            {
                var firstOrDefault = _userRepository.Get(user.Id).Roles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var roleId = firstOrDefault.RoleId;
                    user.Role = _roleManager.FindById(roleId).DisplayName;
                }
            }
            return users;
        }

        public UserDto GetUser(int id)
        {
            
            var user = _userRepository.Get(id);
             return new UserDto()
            {
                Id = user.Id,
                FirstName = user.Name,
                LastName = user.Surname,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Email = user.EmailAddress,
                UserName = user.UserName,
                LastLoginTime = user.LastLoginTime,
                Role = _roleManager.FindById(user.Roles.FirstOrDefault().RoleId).DisplayName
            };
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
            var a = await GetCurrentUserAsync();
            var output = new GetCurrentLoginInformationsOutput
            {
                User = (await GetCurrentUserAsync()).MapTo<UserLoginInfoDto>()
                
            };
            
            return output;
        }
    }
}
