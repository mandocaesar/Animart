using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Abp.Application.Services.Dto;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.Web.Models
{
    public class ResetPasswordViewModel : IInputDto
    {
        
        [EmailAddress]
        [StringLength(Users.User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(Users.User.MaxPlainPasswordLength)]
        public string Password { get; set; }

        [Required]
        [StringLength(Users.User.MaxPlainPasswordLength)]
        public string ConfirmPassword { get; set; }


        [StringLength(Users.User.MaxPasswordResetCodeLength)]
        public string ResetCode { get; set; }

        public long UserId { get; set; }

    }
}