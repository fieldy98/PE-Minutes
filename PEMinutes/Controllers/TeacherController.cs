using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PEMinutes.EF;
using PEMinutes.ViewModels;

//
// OVERVIEW:
//      This controller handles functions for the Teacher role. 
//      They are allowed to Create, View and Edit their minutes.
//      They are also able to approve minutes submitted by their Sub.
//

namespace PEMinutes.Controllers
{
    // This is an extension method to allow me to create the start of the week.
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }
    public class TeacherController : Controller
    {
        private RenExtractEntities ren = new RenExtractEntities();
        private PEMinutesEntities db = new PEMinutesEntities();

        // GET: Teacher
        // Landing Page
        public ActionResult Index()
        {
            var EnteredBadgeString = User.Identity.Name; 
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;

            ViewBag.NeedsApproval = db.SubMinutes.Where(i => i.BadgeNumber == BadgeNumber && i.IsApproved == null).Count();  // tells teacher how many entries need approval

            DateTime now = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(-14);

            TeacherIndexViewModel tivm = new TeacherIndexViewModel();
            EnteredPeMinute epm = new EnteredPeMinute();
            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == BadgeNumber && i.InstructionTime >= end.Date && i.InstructionTime < now.Date).ToList(); // Finds all of the teachers minutes for the last 2 weeks

            List<MinuteCount> MinCount = new List<MinuteCount>();

            for (int i = -14; i < 0; i++)  // for loop to create the MinCount list that tells us how many minutes per day and the date
            {
                MinuteCount mc = new MinuteCount();
                DateTime startday = DateTime.Today.AddDays(i);
                DateTime nextday = DateTime.Today.AddDays(i + 1);
                mc.Date = startday.ToShortDateString();
                foreach(var item in db.EnteredPeMinutes.Where(x => x.InstructionTime >= startday && x.InstructionTime < nextday)) // finding the entry in enteredpeminutes for the given day
                {
                    mc.Minutes = item.Minutes;
                }
                
                
                tivm.MinCount.Add(mc);
            }
            tivm.Minutes = TeachersPeMinutes.Sum(x=>x.Minutes);
            tivm.MinCount = tivm.MinCount.ToList();

            return View(tivm);
        }
        

        public ActionResult Manage()
        {
            DateTime now = DateTime.Now;
            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);

            var ThisMonth = DateTime.Today.ToString("MMMM");
            ViewBag.ThisMonth = ThisMonth;

            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int EnteredBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;

            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == EnteredBadgeNumber && i.InstructionTime > lastDayLastMonth).OrderByDescending(i => i.InstructionTime).ToList(); // Finds the minutes for the signed in teacher for the current month
            ViewBag.CurrentMonthMinuteCount = TeachersPeMinutes.Count();
            ViewBag.TotalMonthMins = TeachersPeMinutes.Sum(x => x.Minutes);
            return View(TeachersPeMinutes);
        }


        // GET: Teacher/Details/5
        public ActionResult Details(int? id)
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int EnteredBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
            if (enteredPeMinute == null)
            {
                return HttpNotFound();
            }
            return View(enteredPeMinute);
        }

        // GET: Teacher/Create
         // Create New Minutes
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,InstructionTime,SubstituteName,IsApproved,ApprovedBy,ApproveTime,InstructionTime")] EnteredPeMinute enteredPeMinute)
        {
            if (ModelState.IsValid)
            {
                var EnteredBadgeString = User.Identity.Name;
                int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int

                // Associate Badge => Staff
                SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);

                // Build variable with information not gathered from user.
                var TeacherNameVariable = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
                enteredPeMinute.TeacherName = TeacherNameVariable;
                enteredPeMinute.School      = SelectedTeacher.Organization_Name;
                enteredPeMinute.Grade       = SelectedTeacher.COURSE_TITLE;
                enteredPeMinute.BadgeNumber = BadgeNumber;
                enteredPeMinute.Timestamp   = DateTime.Now;

                // Apply the modifications and then save to the database
                db.EnteredPeMinutes.Add(enteredPeMinute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enteredPeMinute);
        }

        // GET: Teacher/Approve
        public ActionResult Approve()
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int TeacherBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            ViewBag.School = SelectedTeacher.Organization_Name;

            // Get Teachers minutes that need approval from the SubMinutes Table.
            List<SubMinute> SubMinutesForApproval = db.SubMinutes.Where(i => i.BadgeNumber == TeacherBadgeNumber && i.IsApproved == null).OrderByDescending(i => i.InstructionTime).ToList();
            return View(SubMinutesForApproval);
        }

        // POST: Approve
        [HttpPost]
        public ActionResult Approve(int SelectedID)
        {
            var EnteredBadgeString = User.Identity.Name;
            int TeacherBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            var TeacherNameVariable = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            var Minute = db.SubMinutes.FirstOrDefault(x => x.ID == SelectedID);
            Minute.IsApproved = 1;
            Minute.ApprovedBy = TeacherNameVariable;
            Minute.ApproveTime = DateTime.Now;
            db.Entry(Minute).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true });
        }


        // POST: MoveSubToTeacher
        [HttpPost]
        public ActionResult MoveSubToTeacher(string SelectedSubstituteName, int SelectedMinutes, string SelectedActivity, DateTime SelectedInstructionTime, EnteredPeMinute enteredPeMinute)
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            var TeacherNameVariable = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;

            enteredPeMinute.TeacherName = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            enteredPeMinute.Minutes = SelectedMinutes;
            enteredPeMinute.Activity = SelectedActivity;
            enteredPeMinute.Timestamp = DateTime.Now;

            enteredPeMinute.InstructionTime = SelectedInstructionTime;


            enteredPeMinute.SubstituteName = SelectedSubstituteName;
            int BadgeNumber = Int32.Parse(SelectedTeacher.BADGE_NUM);  // convert string to int
            enteredPeMinute.BadgeNumber = BadgeNumber;
            enteredPeMinute.School = SelectedTeacher.Organization_Name;
            enteredPeMinute.Grade = SelectedTeacher.COURSE_TITLE;
            
            enteredPeMinute.IsApproved = 1;
            enteredPeMinute.ApprovedBy = TeacherNameVariable;
            enteredPeMinute.ApproveTime = DateTime.Now;
            db.EnteredPeMinutes.Add(enteredPeMinute);
            db.SaveChanges();

            return Json(new { success = true });
        }

        // GET: Teacher/Edit/5
        // Edit Minutes
        public ActionResult Edit(int? id)
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int EnteredBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
            if (enteredPeMinute == null)
            {
                return HttpNotFound();
            }
            TeacherEditViewModel tevm = new TeacherEditViewModel();
            tevm.ID = enteredPeMinute.ID;
            tevm.Minutes = enteredPeMinute.Minutes;
            tevm.InstructionTime = enteredPeMinute.InstructionTime;
            tevm.Activity = enteredPeMinute.Activity;
            tevm.TeacherName = enteredPeMinute.TeacherName;
            tevm.SubstituteName = enteredPeMinute.SubstituteName;
            tevm.ApprovedBy = enteredPeMinute.ApprovedBy;
            tevm.BadgeNumber = enteredPeMinute.BadgeNumber;
            tevm.Grade = enteredPeMinute.Grade;
            tevm.IsApproved = enteredPeMinute.IsApproved;
            tevm.School = enteredPeMinute.School;
            tevm.Timestamp = enteredPeMinute.Timestamp;
            tevm.ApprovedTime = enteredPeMinute.ApproveTime;
            return View(tevm);
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime,InstructionTime")] EnteredPeMinute enteredPeMinute)
        {
            if (ModelState.IsValid)
            {
                enteredPeMinute.IsApproved = 1;
                enteredPeMinute.ApprovedBy = enteredPeMinute.TeacherName;
                enteredPeMinute.ApproveTime = DateTime.Now;
                db.Entry(enteredPeMinute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enteredPeMinute);
        }

        // GET: Teacher/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
            if (enteredPeMinute == null)
            {
                return HttpNotFound();
            }
            return View(enteredPeMinute);
        }

        // POST: Teacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
            db.EnteredPeMinutes.Remove(enteredPeMinute);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}







//public JsonResult IdentifyTeacher(string EnteredBadge)
//{
//    var teacherbadge = ren.SchoolTeachersWithADLogins.Where(x => x.BADGE_NUM == EnteredBadge).FirstOrDefault();
//    if (EnteredBadge == teacherbadge.BADGE_NUM)
//    {
//        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
//    }
//    else
//    {
//        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
//    }
//}


