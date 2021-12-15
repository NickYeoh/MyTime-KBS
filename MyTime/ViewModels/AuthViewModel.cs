using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;

namespace MyTime.ViewModels
{
    public class AuthViewModel
    {
        public UserModel User { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}