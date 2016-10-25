using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PEMinutes.ViewModels
{
    public class SubstituteViewModel
    {
        [Required]
        
        public string SubstituteName { get; set; }

        [Required]
        public string InstructionTime { get; set; }

        [Required]
        public int? Minutes { get; set; }

        [Required]
        public string Activity { get; set; }

        public string BadgeNumber { get; set; }
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public List<SchList> SchoolList { get; set; }
        public List<TeachList> TeacherList { get; set; }
        public SubstituteViewModel()
        {
            SchoolList = new List<SchList>();
            TeacherList = new List<TeachList>();
        }
    }

    public class SchList
    { 
        public string SchoolName { get; set; }

    }
    public class TeachList
    {
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string BadgeNumber { get; set; }

    }
}