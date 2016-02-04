using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Animart.Portal.Web.Models
{
    public class RegisterResultViewModel
    {
        public string TenancyName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string NameAndSurname { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailConfirmationRequired { get; set; }
    }
}