using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PEMinutes.EF;
using System.Web.Mvc;

namespace PEMinutes.ViewModels
{
    public class PrincipalViewModel
    {
        public string TeacherName { get; set; }
        public string Activity { get; set; }
        public string Timestamp { get; set; }
        public string Minutes { get; set; }

        public IEnumerable<SelectListItem> SchoolTeachers { get; set; }

    }
}
