using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Animart.Portal.Web.Models
{
    public class ForgotPasswordResultViewModel
    { 
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmationRequired { get; set; }
    }
}