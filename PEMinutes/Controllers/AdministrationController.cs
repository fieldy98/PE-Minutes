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
            var startDay = _db.EnteredPeMinutes.Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).FirstOrDefault().Value.Date;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                var date = Convert.ToDateTime(selectedDate);
                startDay = date.Date;
            }
            var tenEntryDaysBack = _db.EnteredPeMinutes.Where(x => x.InstructionTime <= startDay).Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).Take(10).LastOrDefault().Value.Date;
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
