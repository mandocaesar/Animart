using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Animart.Portal.Web.Models
{
    public class ResetPasswordResultViewModel
    { 
        public string EmailAddress { get; set; }
        public bool IsSuccess { get; set; }
    }
}