using System.Collections.Generic;

namespace PEMinutes.ViewModels
{
    public class PrincipalIndexViewModel
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public List<MeetingReq> MeetReq { get; set; }
        public List<NotMeetingReq> NotReq { get; set; }
        public string Date { get; set; }
        public List<Graphing> Graph { get; set; }
        public List<PrinicipalReports> ListReports { get; set; }
        public PrincipalIndexViewModel()
        {
            MeetReq = new List<MeetingReq>();
            NotReq = new List<NotMeetingReq>();
            Graph = new List<Graphing>();
            ListReports = new List<PrinicipalReports>();
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

    public class PrinicipalReports
    {
        public string TeacherName { get; set; }
        public string InstructionTime { get; set; }
        public int? Minutes { get; set; }

    }
}
