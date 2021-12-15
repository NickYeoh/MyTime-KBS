using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class ChangePasswordModel
    {

        [DataType(DataType.Password)]
        [Display(Name = "PasswordNew", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PasswordNewRequired")]
        [MaxLength(20, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string PasswordNew { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirm", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PasswordConfirmRequired")]
        [MaxLength(20, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        [Compare("PasswordNew", ErrorMessageResourceName = "InvalidPasswordConfirm", ErrorMessageResourceType = typeof(Resource))]
        public string PasswordConfirm { get; set; }

    }
}