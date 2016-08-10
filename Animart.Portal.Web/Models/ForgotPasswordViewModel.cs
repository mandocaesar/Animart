using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Abp.Application.Services.Dto;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.Web.Models
{
    public class ForgotPasswordViewModel : IInputDto
    {
        [Required]
        [EmailAddress]
        [StringLength(Users.User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}