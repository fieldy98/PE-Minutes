using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PEMinutes.EF;
using PEMinutes.Models;
using PEMinutes.ViewModels.Teacher;
using System.Globalization;

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
            DateTime now = DateTime.Now;

            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month

            var EnteredBadgeString = User.Identity.Name; 
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            ViewBag.School = SelectedTeacher.Organization_Name;
            ViewBag.NeedsApproval = db.SubMinutes.Where(i => i.BadgeNumber == BadgeNumber && i.IsApproved == null).Count();
            ViewBag.Approved = db.EnteredPeMinutes.Where(i => i.BadgeNumber == BadgeNumber && i.ApprovedBy != null && i.Timestamp > lastDayLastMonth).Count();

            DateTime CurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var CurrentWeekMinutes = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= CurrentWeek).Sum(x => x.Minutes);
            if (CurrentWeekMinutes == null)
            {
                ViewBag.CurrentWeekMinutes = 0;
            }
            else
            {
                ViewBag.CurrentWeekMinutes = CurrentWeekMinutes;
            }

            var ThisMonth = DateTime.Today.ToString("MMMM");
            ViewBag.ThisMonth = ThisMonth;

            // this is a hard coded entry for the teachrs chart so that they can visually see if they missed a day. The other method that is used for admin view will skip the non entered days on the label and the teachers wont be able to see what they missed easily
            var FourteenDaysAgo = DateTime.Now.AddDays(-14).Date;
            var ThirteenDaysAgo = DateTime.Now.AddDays(-13).Date;
            var TwelveDaysAgo = DateTime.Now.AddDays(-12).Date;
            var ElevenDaysAgo = DateTime.Now.AddDays(-11).Date;
            var TenDaysAgo = DateTime.Now.AddDays(-10).Date;
            var NineDaysAgo = DateTime.Now.AddDays(-9).Date;
            var EightDaysAgo = DateTime.Now.AddDays(-8).Date;
            var SevenDaysAgo = DateTime.Now.AddDays(-7).Date;
            var SixDaysAgo = DateTime.Now.AddDays(-6).Date;
            var FiveDaysAgo = DateTime.Now.AddDays(-5).Date;
            var FourDaysAgo = DateTime.Now.AddDays(-4).Date;
            var ThreeDaysAgo = DateTime.Now.AddDays(-3).Date;
            var TwoDaysAgo = DateTime.Now.AddDays(-2).Date;
            var Yesterday = DateTime.Now.AddDays(-1).Date;
            var Today = DateTime.Today.Date;

            ViewBag.FourteenDays = FourteenDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.ThirteenDays = ThirteenDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.TwelveDays = TwelveDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.ElevenDays = ElevenDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.TenDays = TenDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.NineDays = NineDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.EightDays = EightDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.SevenDays = SevenDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.SixDays = SixDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.FiveDays = FiveDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.FourDays = FourDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.ThreeDays = ThreeDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.TwoDays = TwoDaysAgo.ToString("MMM dd,yyyy");
            ViewBag.Yester = Yesterday.ToString("MMM dd,yyyy");
            ViewBag.Now = Today.ToString("MMM dd,yyyy");
            ViewBag.FourteenDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= FourteenDaysAgo && x.Timestamp < ThirteenDaysAgo).Sum(x => x.Minutes);
            ViewBag.ThirteenDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= ThirteenDaysAgo && x.Timestamp < TwelveDaysAgo).Sum(x => x.Minutes);
            ViewBag.TwelveDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= TwelveDaysAgo && x.Timestamp < ElevenDaysAgo).Sum(x => x.Minutes);
            ViewBag.ElevenDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= ElevenDaysAgo && x.Timestamp < TenDaysAgo).Sum(x => x.Minutes);
            ViewBag.TenDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= TenDaysAgo && x.Timestamp < NineDaysAgo).Sum(x => x.Minutes);
            ViewBag.NineDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= NineDaysAgo && x.Timestamp < EightDaysAgo).Sum(x => x.Minutes);
            ViewBag.EightDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= EightDaysAgo && x.Timestamp < SevenDaysAgo).Sum(x => x.Minutes);
            ViewBag.SevenDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= SevenDaysAgo && x.Timestamp < SixDaysAgo).Sum(x => x.Minutes);
            ViewBag.SixDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= SixDaysAgo && x.Timestamp < FiveDaysAgo).Sum(x => x.Minutes);
            ViewBag.FiveDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= FiveDaysAgo && x.Timestamp < FourDaysAgo).Sum(x => x.Minutes);
            ViewBag.FourDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= FourDaysAgo && x.Timestamp < ThreeDaysAgo).Sum(x => x.Minutes);
            ViewBag.ThreeDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= ThreeDaysAgo && x.Timestamp < TwoDaysAgo).Sum(x => x.Minutes);
            ViewBag.TwoDaysAgo = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= TwoDaysAgo && x.Timestamp < Yesterday).Sum(x => x.Minutes);
            ViewBag.Yesterday = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= Yesterday && x.Timestamp < Today).Sum(x => x.Minutes);
            ViewBag.Today = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= Today).Sum(x => x.Minutes);
            // end of the sections where we build the chart data

            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == BadgeNumber).ToList(); // Finds all of the teachers minutes

            return View(TeachersPeMinutes);
        }
        

        public ActionResult Manage()
        {
            DateTime now = DateTime.Now;

            var ThisMonth = DateTime.Today.ToString("MMMM");
            ViewBag.ThisMonth = ThisMonth;

            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month

            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int EnteredBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == EnteredBadgeNumber && i.Timestamp > lastDayLastMonth).OrderByDescending(i => i.Timestamp).ToList(); // Finds the minutes for the signed in teacher for the current month
            var Approved = db.EnteredPeMinutes.Where(i => i.BadgeNumber == EnteredBadgeNumber && i.SubstituteName != null && i.Timestamp > lastDayLastMonth).Count();
            ViewBag.Approved = Approved;
            ViewBag.CurrentMonthMinuteCount = TeachersPeMinutes.Count();
            ViewBag.PercentApproved = ((float)Approved / TeachersPeMinutes.Count()).ToString("0.00%");
            ViewBag.TotalMonthMins = TeachersPeMinutes.Sum(x => x.Minutes);
            return View(TeachersPeMinutes);
        }


        // GET: Teacher/Details/5
        public ActionResult Details(int? id)
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

        // GET: Teacher/Create
         // Create New Minutes
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] EnteredPeMinute enteredPeMinute)
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
            List<SubMinute> SubMinutesForApproval = db.SubMinutes.Where(i => i.BadgeNumber == TeacherBadgeNumber && i.IsApproved == null).OrderByDescending(i => i.Timestamp).ToList();
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


        // POST: Checkout 
        [HttpPost]
        public ActionResult MoveSubToTeacher(string SelectedSubstituteName, int SelectedMinutes, string SelectedActivity, DateTime SelectedTimestamp, EnteredPeMinute enteredPeMinute)
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            var TeacherNameVariable = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;

            enteredPeMinute.TeacherName = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            enteredPeMinute.Minutes = SelectedMinutes;
            enteredPeMinute.Activity = SelectedActivity;
            enteredPeMinute.Timestamp = SelectedTimestamp;
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

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] EnteredPeMinute enteredPeMinute)
        {
            if (ModelState.IsValid)
            {
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


