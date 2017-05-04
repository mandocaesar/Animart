using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Animart.Portal.Web.Models
{
    public class LoginFormViewModel
    {
        public string ReturnUrl { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }
    }
}