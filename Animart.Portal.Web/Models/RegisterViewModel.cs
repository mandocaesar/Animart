using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Abp.Application.Services.Dto;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.Web.Models
{
    public class RegisterViewModel : IInputDto
    {
        /// <summary>
        /// Not required for single-tenant applications.
        /// </summary>
        [StringLength(Tenant.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(Users.User.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(Users.User.MaxSurnameLength)]
        public string Surname { get; set; }

        [StringLength(Users.User.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(Users.User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(Users.User.MaxPlainPasswordLength)]
        public string Password { get; set; }

        public bool IsExternalLogin { get; set; }

        public RegisterViewModel()
        {
            TenancyName = "Animart";
        }
    }
}