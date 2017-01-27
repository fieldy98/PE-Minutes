using System;
using System.Linq;
using System.Net;
using PEMinutes.ViewModels;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PEMinutes.EF;

namespace PEMinutes.Controllers
{
    public class PrincipalController : Controller
    {
        private readonly PEMinutesEntities _db = new PEMinutesEntities();
        private readonly RenExtractEntities _ren = new RenExtractEntities();

        // GET: /Principal/Index
        public ActionResult Index(string selectedDate)
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedPrincipal = _ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var selectedSchool = selectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = selectedPrincipal.Principal;

            var tday = _ren.TeachableDays.OrderByDescending(x => x.TeachableDays).Take(10);
            var startDay = tday.First().TeachableDays; ;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                var date = Convert.ToDateTime(selectedDate);
                startDay = date.Date;
            }
            var tenEntryDaysBack = _ren.TeachableDays.Where(x => x.TeachableDays <= startDay).OrderByDescending(x => x.TeachableDays).Take(10).ToList().Last().TeachableDays;
            var principalView = _ren.SchoolTeachersWithADLogins.Where(x => x.Organization_Name == selectedSchool && x.COURSE_TITLE != "Kindergarten" && x.COURSE_TITLE != "PS - 6th SpEd").ToList();  // select all minutes from the school the principal belongs to
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

            pivm.Date = startDay.ToShortDateString();
            pivm.DateStart = tenEntryDaysBack.ToShortDateString();
            pivm.DateEnd = startDay.ToShortDateString();
            pivm.MeetReq = pivm.MeetReq.ToList();
            pivm.NotReq = pivm.NotReq.ToList();
            pivm.Graph = pivm.Graph.ToList();
            // end of the data for the graph in admin view
            return View(pivm);
        }

        // GET: /Principal/Reports
        public ActionResult Reports()
        {
            PrincipalIndexViewModel pivm = new PrincipalIndexViewModel();
            var enteredBadgeString = User.Identity.Name;
            var selectedPrincipal = _ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var selectedSchool = selectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = selectedPrincipal.Principal;

            var tday = _ren.TeachableDays.OrderByDescending(x => x.TeachableDays).Take(10);
            var startDay = tday.First().TeachableDays;
            var teacherlist = _ren.SchoolTeachersWithADLogins.Where(x => x.Organization_Name == selectedSchool && x.COURSE_TITLE != "Kindergarten" && x.COURSE_TITLE != "PS - 6th SpEd").ToList();
            var pastTenDays = _ren.TeachableDays.Where(x => x.TeachableDays <= startDay).OrderByDescending(x => x.TeachableDays).Take(10).ToList().Last().TeachableDays;
            var schoolReport = _db.EnteredPeMinutes.Where(x => x.School == selectedSchool && x.InstructionTime > pastTenDays).OrderBy(x => x.Minutes); // select all minutes from the school the principal belongs to

            foreach (var item in schoolReport)
            {
                PrinicipalReports pr = new PrinicipalReports
                {
                    TeacherName = item.TeacherName,
                    Minutes = item.Minutes
                };

                var time = Convert.ToDateTime(item.InstructionTime);
                pr.InstructionTime = time.ToShortDateString();

                pivm.ListReports.Add(pr);
            }

            foreach (var item in teacherlist)
            {
                var name = item.TeacherFirstName + " " + item.TeacherLastName;
                PrincipalSum rv = new PrincipalSum
                {
                    TeacherName = name,
                    Minutes = schoolReport.Where(x => x.TeacherName == name).Sum(x => x.Minutes)
                };
                if (rv.Minutes == null)
                { rv.Minutes = 0; }

                rv.Percentage = ((float)rv.Minutes / 2) + "%";

                pivm.Reports.Add(rv);
            }

            pivm.Reports = pivm.Reports.ToList();
            pivm.ListReports = pivm.ListReports.ToList();

            return View(pivm);
        }

        // GET: Principal/Details/5
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
