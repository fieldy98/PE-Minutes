using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEMinutes.ViewModels.Teacher
{
    public class TeacherIndexViewModel
    {
        public string Name { get; set; }
        public string School { get; set; }
        public int NeedsApproval { get; set; }
        public int Approved { get; set; }


        // Time Variables used for graph
        public string CurrentWeekMinutes { get; set; }
        public string ThisMonth { get; set; }
        public string FourteenDays { get; set; }
        public string ThirteenDays { get; set; }
        public string TwelveDays { get; set; }
        public string ElevenDays { get; set; }
        public string TenDays { get; set; }
        public string NineDays { get; set; }
        public string EightDays { get; set; }
        public string SevenDays { get; set; }
        public string SixDays { get; set; }
        public string FiveDays { get; set; }
        public string FourDays { get; set; }
        public string ThreeDays { get; set; }
        public string TwoDays { get; set; }
        public string Yester { get; set; }
        public string Now { get; set; }
        public string FourteenDaysAgo { get; set; }
        public string ThirteenDaysAgo { get; set; }
        public string TwelveDaysAgo { get; set; }
        public string ElevenDaysAgo { get; set; }
        public string TenDaysAgo { get; set; }
        public string NineDaysAgo { get; set; }
        public string EightDaysAgo { get; set; }
        public string SevenDaysAgo { get; set; }
        public string SixDaysAgo { get; set; }
        public string FiveDaysAgo { get; set; }
        public string FourDaysAgo { get; set; }
        public string ThreeDaysAgo { get; set; }
        public string TwoDaysAgo { get; set; }
        public string Yesterday { get; set; }
        public string Today { get; set; }
      


    }
}