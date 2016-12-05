﻿using System;
using System.Linq;
using System.Net;
using PEMinutes.ViewModels;
using System.Web.Mvc;
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
            var startDay = DateTime.Now;
            if(!string.IsNullOrEmpty(selectedDate))
            {
                var date = Convert.ToDateTime(selectedDate);
                startDay = date.Date;
            }
            
            var currentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var lastweek = startDay.AddDays(-14);
            ViewBag.lastweek = lastweek;  // used to find falling behind teachers
            ViewBag.CurrentWeek = currentWeek; // used to find falling behind teachers
            var enteredBadgeString = User.Identity.Name;
            var selectedAdmin = _ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedAdmin.FIRST_NAME + " " + selectedAdmin.LAST_NAME;
            var adminView = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= lastweek.Date && x.InstructionTime < startDay.Date && x.School.Contains("Elem")).OrderBy(x => x.School); // select all minutes from the school the principal belongs to
            var avm = new AdministrationViewModel();
            var numberTeachers = _ren.PEMinutesTeacherCounts.Where(x=>x.Organization_Name.Contains("Elem")).ToList();

            foreach (var item in numberTeachers)
            {
                var tc = new TeacherCount();
                var schoolfullname = item.Organization_Name;
                tc.CountTeacher = item.TEACHER;
                var schoolname = schoolfullname.Substring(0, schoolfullname.Length - 18);
                tc.SchoolName = schoolname;
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
                tc.Percent = ((float)count / item.TEACHER) *100;
                avm.TeachCount.Add(tc);
            }
            avm.Date = startDay.ToShortDateString();
            avm.TeachCount = avm.TeachCount.ToList();
            return View(avm);

        }
        public ActionResult Reports(int timeFrame, int divisor)
        {
            var now = DateTime.Now;
            var selectedTimeFrame = now.AddDays(timeFrame);
            var currentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var lastweek = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
            ViewBag.lastweek = lastweek;  // used to find falling behind teachers
            ViewBag.CurrentWeek = currentWeek; // used to find falling behind teachers
            ViewBag.Divisor = divisor;
            var enteredBadgeString = User.Identity.Name;
            var selectedAdmin = _ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedAdmin.FIRST_NAME + " " + selectedAdmin.LAST_NAME;
            var adminView = _db.EnteredPeMinutes.Where(x => x.InstructionTime >= selectedTimeFrame.Date && x.InstructionTime < now.Date).OrderBy(x => x.School); // select all minutes from the school the principal belongs to
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
