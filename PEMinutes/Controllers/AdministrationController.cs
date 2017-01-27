using System;
using System.Linq;
using System.Net;
using PEMinutes.ViewModels;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PEMinutes.EF;

namespace PEMinutes.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly PEMinutesEntities _db = new PEMinutesEntities();
        private readonly RenExtractEntities _ren = new RenExtractEntities();

        // GET: Administration
        public ActionResult Index(string selectedDate)
        {
            var tday = _ren.TeachableDays.OrderByDescending(x => x.TeachableDays).Take(10);


            var startDay = tday.First().TeachableDays;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                var date = Convert.ToDateTime(selectedDate);
                startDay = date.Date;
            }
            var tenEntryDaysBack = tday.ToList().Last().TeachableDays;
            var enteredBadgeString = User.Identity.Name;
            var selectedAdmin = _ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedAdmin.FIRST_NAME + " " + selectedAdmin.LAST_NAME;
            var adminView = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= tenEntryDaysBack && x.InstructionTime <= startDay && x.School.Contains("Elem")).OrderBy(x => x.School); // select all minutes from the school the principal belongs to
            var avm = new AdministrationViewModel();
            var numberTeachers = _ren.PEMinutesTeacherCounts.Where(x => x.Organization_Name.Contains("Elem")).ToList();

            foreach (var item in numberTeachers)
            {
                var tc = new TeacherCount();
                var schoolfullname = item.Organization_Name;
                tc.TotalTeachers = item.TEACHER;
                var schoolname = schoolfullname.Substring(0, schoolfullname.Length - 18);
                tc.ShortSchoolName = schoolname;
                var count = 0;
                foreach (var teach in adminView.Where(x => x.School == item.Organization_Name).GroupBy(x => x.TeacherName))
                {
                    var sum = teach.Sum(x => x.Minutes);
                    if (sum >= 200)
                    {
                        count++;

                    }
                }
                tc.MeetReq = count;
                tc.Percent = ((float)count / item.TEACHER) * 100;
                tc.Percent = (float)Math.Round((double)tc.Percent, 2);
                avm.TeachCount.Add(tc);
            }
            avm.Date = startDay.ToShortDateString();
            avm.DateStart = tenEntryDaysBack.ToShortDateString();
            avm.DateEnd = startDay.ToShortDateString();
            avm.TeachCount = avm.TeachCount.ToList();
            return View(avm);

        }

        public ActionResult SchoolView(string selectedDate, string schoolName)
        {

            var startDay = _db.EnteredPeMinutes.Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).FirstOrDefault().Value.Date;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                var date = Convert.ToDateTime(selectedDate);
                startDay = date.Date;
            }
            var tenEntryDaysBack = _db.EnteredPeMinutes.Where(x => x.InstructionTime <= startDay).Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).Take(10).LastOrDefault().Value.Date;
            var principalView = _ren.SchoolTeachersWithADLogins.Where(x => x.Organization_Name == schoolName && x.COURSE_TITLE != "Kindergarten" && x.COURSE_TITLE != "PS - 6th SpEd").ToList(); // select all minutes from the school the principal belongs to
            var pivm = new PrincipalIndexViewModel();

            foreach (var item in principalView)
            {
                var mr = new MeetingReq();
                var name = item.TeacherFirstName + " " + item.TeacherLastName;
                var sumMinutes = _db.EnteredPeMinutes.Where(x => x.InstructionTime > tenEntryDaysBack && x.InstructionTime <= startDay && x.TeacherName == name).Sum(x => x.Minutes);
                if (!(sumMinutes >= 200)) continue;
                mr.TeacherName = name;
                mr.Minutes = sumMinutes;
                pivm.MeetReq.Add(mr);
            }

            foreach (var item in principalView)
            {
                var nmr = new NotMeetingReq();
                var name = item.TeacherFirstName + " " + item.TeacherLastName;
                var sumMinutes = _db.EnteredPeMinutes.Where(x => x.InstructionTime > tenEntryDaysBack && x.InstructionTime <= startDay && x.TeacherName == name).Sum(x => x.Minutes);
                if (!(sumMinutes < 200 || sumMinutes == null)) continue;
                nmr.TeacherName = name;
                nmr.Minutes = sumMinutes;
                if (sumMinutes == null)
                { nmr.Minutes = 0; }
                pivm.NotReq.Add(nmr);
            }

            foreach (var item in principalView)
            {
                var g = new Graphing();
                var name = item.TeacherFirstName + " " + item.TeacherLastName;
                var sumMinutes = _db.EnteredPeMinutes.Where(x => x.InstructionTime > tenEntryDaysBack && x.InstructionTime <= startDay && x.TeacherName == name).Sum(x => x.Minutes);
                g.TeacherName = name;
                g.Minutes = sumMinutes;
                pivm.Graph.Add(g);
            }
            pivm.School = schoolName;
            pivm.Date = startDay.ToShortDateString();
            pivm.DateStart = tenEntryDaysBack.ToShortDateString();
            pivm.DateEnd = startDay.ToShortDateString();
            pivm.MeetReq = pivm.MeetReq.ToList();
            pivm.NotReq = pivm.NotReq.ToList();
            pivm.Graph = pivm.Graph.ToList();
            // end of the data for the graph in admin view
            return View(pivm);
        }

        public ActionResult Reports()
        {
            AdministrationViewModel avm = new AdministrationViewModel();
            var now = _db.EnteredPeMinutes.Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).FirstOrDefault().Value.Date;
            var selectedTimeFrame = _db.EnteredPeMinutes.Where(x => x.InstructionTime <= now).Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).Take(10).LastOrDefault().Value.Date;
            var enteredBadgeString = User.Identity.Name;
            var selectedAdmin = _ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedAdmin.FIRST_NAME + " " + selectedAdmin.LAST_NAME;
            var sumReports = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= selectedTimeFrame.Date && x.InstructionTime < now.Date).OrderBy(x => x.School).DistinctBy(x => x.TeacherName).ToList(); // select all minutes from the school the principal belongs to
            var allReports = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= selectedTimeFrame.Date && x.InstructionTime < now.Date).OrderBy(x => x.School).ToList();

            foreach (var item in sumReports)
            {
                ReportView rv = new ReportView
                {
                    TeacherName = item.TeacherName,
                    SchoolName = item.School.Substring(0, item.School.Length - 18),
                    Minutes = allReports.Where(x => x.TeacherName == item.TeacherName).Sum(x => x.Minutes)
                };

                rv.Percentage = ((float)rv.Minutes / 2) + "%";

                avm.Reports.Add(rv);
            }

            foreach (var item in allReports)
            {
                ReportList rl = new ReportList
                {
                    TeacherName = item.TeacherName,
                    SchoolName = item.School.Substring(0, item.School.Length - 18),
                    Minutes = item.Minutes
                };

                var time = Convert.ToDateTime(item.InstructionTime);
                rl.InstructionTime = time.ToShortDateString();

                avm.ListReports.Add(rl);
            }

            avm.Reports = avm.Reports.ToList();
            avm.ListReports = avm.ListReports.ToList();

            return View(avm);
        }

        // GET: Administration/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enteredPeMinute = _db.EnteredPeMinutes.Find(id);
            if (enteredPeMinute == null)
            {
                return HttpNotFound();
            }
            return View(enteredPeMinute);
        }
    }
}
