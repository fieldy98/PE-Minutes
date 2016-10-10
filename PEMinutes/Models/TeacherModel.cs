using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEMinutes.Models
{
    public class TeacherModel
    {
        public string TeacherName { get; set;}
        public int Minutes { get; set; }
        public int BadgeNumber { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Activity { get; set; }
        public DateTime Timestamp { get; set; }

        // Needed to approved Sub Minutes
        public string SubstituteName { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApproveTime { get; set; }

    }
}