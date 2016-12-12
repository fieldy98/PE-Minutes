using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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

            var lastweek = startDay.AddDays(-14);
            
            var TenEntryDaysBack  = _db.EnteredPeMinutes.Where(x=>x.InstructionTime <= startDay).Select(x=>x.InstructionTime).DistinctBy(x=>x.Value.Date).OrderByDescending(x => x).Take(10).LastOrDefault().Value.Date;
            var enteredBadgeString = User.Identity.Name;
            var selectedAdmin = _ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedAdmin.FIRST_NAME + " " + selectedAdmin.LAST_NAME;
            var adminView = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= TenEntryDaysBack && x.InstructionTime <= startDay && x.School.Contains("Elem")).OrderBy(x => x.School); // select all minutes from the school the principal belongs to
            var avm = new AdministrationViewModel();
            var numberTeachers = _ren.PEMinutesTeacherCounts.Where(x=>x.Organization_Name.Contains("Elem")).OrderBy(x => x.Organization_Name).ToList();







            foreach (var item in numberTeachers)
            {
                var tc = new TeacherCount();
                tc.TotalTeachers = item.TEACHER;
                tc.ShortSchoolName = item.Organization_Name.Substring(0, item.Organization_Name.Length - 18);
                var count = 0;
                foreach (var teacher in adminView.Where(x => x.School == item.Organization_Name).GroupBy(x => x.BadgeNumber))
                {
                    var sum = teacher.Sum(x => x.Minutes);
                    if (sum >= 200)
                    {
                        count++;

                    }
                }
                tc.MeetReq = count;
                tc.Percent = (((float)count / item.TEACHER) * 100);
                avm.TeachCount.Add(tc);
            }
            avm.Date = startDay.ToShortDateString();
            avm.TeachCount = avm.TeachCount.ToList();
            return View(avm);

        }
        public ActionResult Reports(int timeFrame, int divisor)
        {

            
            var startDay = _db.EnteredPeMinutes.Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).FirstOrDefault().Value.Date;
            var selectedTimeFrame = _db.EnteredPeMinutes.Where(x => x.InstructionTime <= startDay).Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).Take(timeFrame).LastOrDefault().Value.Date;
            var currentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var lastweek = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
            ViewBag.lastweek = lastweek;  // used to find falling behind teachers
            ViewBag.CurrentWeek = currentWeek; // used to find falling behind teachers
            ViewBag.Divisor = divisor;
            var enteredBadgeString = User.Identity.Name;
            var selectedAdmin = _ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedAdmin.FIRST_NAME + " " + selectedAdmin.LAST_NAME;
            var adminView = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= selectedTimeFrame.Date && x.InstructionTime < startDay).OrderBy(x => x.School); // select all minutes from the school the principal belongs to
            return View(adminView);
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
