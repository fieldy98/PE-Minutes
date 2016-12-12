﻿using System.Collections.Generic;

namespace PEMinutes.ViewModels
{
    public class AdministrationViewModel
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }
        public string SchoolName { get; set; }
        public string OrgName { get; set; }
        public int? CountTeacher { get; set; }
        public int? MeetReq { get; set; }
        public float? Percent { get; set; }
        public string Date { get; set; }
        public List<TeacherSelectListEntry> MyTeachers { get; set; }
        public List<TeacherCount> TeachCount { get; set; }
        public AdministrationViewModel()
        {
            MyTeachers = new List<TeacherSelectListEntry>();
            TeachCount = new List<TeacherCount>();
        }
    }

    public class TeacherSelectListEntry
    {
        public string TeacherName { get; set; }
        public string SchoolName { get; set; }
        public int? Minutes { get; set; }
        public int? CountTeacher { get; set; }

    }
    //public class TeacherCount
    //{
    //    public string SchoolName { get; set; }
    //    public int? CountTeacher { get; set; }
    //    public int? MeetReq { get; set; }
    //    public float? Percent { get; set; }

    //}
    public class TeacherCount
    {
        public string ShortSchoolName { get; set; }
        public int? SumMinutes { get; set; }
        public int? TotalTeachers { get; set; }
        public int? MeetReq { get; set; }
        public float? Percent { get; set; }

    }
}