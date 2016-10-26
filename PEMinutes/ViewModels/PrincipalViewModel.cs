using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PEMinutes.EF;
using System.Web.Mvc;

namespace PEMinutes.ViewModels
{
    public class PrincipalIndexViewModel
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }
        public List<MeetingReq> MeetReq { get; set; }
        public List<NotMeetingReq> NotReq { get; set; }
        public List<Graphing> Graph { get; set; }
        public PrincipalIndexViewModel()
        {
            MeetReq = new List<MeetingReq>();
            NotReq = new List<NotMeetingReq>();
            Graph = new List<Graphing>();
        }
    }

    public class MeetingReq
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }

    }
    public class NotMeetingReq
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }
    }
    public class Graphing
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }
    }
}
