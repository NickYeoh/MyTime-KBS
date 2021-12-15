using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class ShiftModel
    {

        [Display(Name = "ShiftID", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ShiftIDRequired")]
        public string ShiftID { get; set; }

        [Display(Name = "ShiftName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ShiftNameRequired")]
        public string ShiftName { get; set; }

        public bool IsWorkDay1 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm tt}")]
        public string TimeIn1 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm tt}")]
        public string TimeOut1 { get; set; }

        public int FlexiTimeInterval1 { get; set; }

        public bool IsWorkDay2 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeIn2 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeOut2 { get; set; }

        public int FlexiTimeInterval2 { get; set; }

        public bool IsWorkDay3 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeIn3 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeOut3 { get; set; }

        public int FlexiTimeInterval3 { get; set; }

        public bool IsWorkDay4 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeIn4 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeOut4 { get; set; }

        public int FlexiTimeInterval4 { get; set; }

        public bool IsWorkDay5 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeIn5 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeOut5 { get; set; }

        public int FlexiTimeInterval5 { get; set; }

        public bool IsWorkDay6 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeIn6 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeOut6 { get; set; }

        public int FlexiTimeInterval6 { get; set; }

        public bool IsWorkDay7 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeIn7 { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public string TimeOut7 { get; set; }

        public int FlexiTimeInterval7 { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }

        public bool IsOverNightShift { get; set; }
       

    }
}