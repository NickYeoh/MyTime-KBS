
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace MyTime.Models
{
    public class UserModel
    {
        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "NRICRequired")]
        [MaxLength(20, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        // Device User ID
        public string USRID { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "UserNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(Resource))]
        public string Gender { get; set; }

        [Display(Name = "ContactNo", ResourceType = typeof(Resource))]
        [MaxLength(25, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string ContactNo { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resource))]
        //[DataType(DataType.EmailAddress, ErrorMessageResourceName = "InvalidEmail" , ErrorMessageResourceType = typeof(Resource))]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Resource))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DepartmentNameRequired")]
        public string DepartmentID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UnitID", ResourceType = typeof(Resource))]
        public int? UnitID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(Resource))]
        public string UnitName { get; set; }

        [Display(Name = "Designation", ResourceType = typeof(Resource))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string Designation { get; set; }

        [Display(Name = "Grade", ResourceType = typeof(Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string Grade { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RoleNameRequired")]
        public int RoleID { get; set; }

        [Display(Name = "RoleName", ResourceType = typeof(Resource))]
        public string RoleName { get; set; }

        [Display(Name = "ShiftID", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ShiftNameRequired")]
        public string ShiftID { get; set; }

        [Display(Name = "ShiftName", ResourceType = typeof(Resource))]
        public string ShiftName { get; set; }

        [Display(Name = "IsResigned", ResourceType = typeof(Resource))]
        public bool IsResigned { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "On", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ResignedOn { get; set; }

        [Display(Name = "AccessRoleName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "AccessRoleNameRequired")]
        public int AccessRoleID { get; set; }

        [Display(Name = "AccessRoleName", ResourceType = typeof(Resource))]
        public string AccessRoleName { get; set; }

        public bool IsAttendanceExcluded { get; set; }

        [Display(Name = "AttendanceCardStatus", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus{ get; set; }

    }
}
