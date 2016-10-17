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
using PEMinutes.ViewModels;
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
            var EnteredBadgeString = User.Identity.Name; 
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            ViewBag.School = SelectedTeacher.Organization_Name;
            ViewBag.NeedsApproval = db.SubMinutes.Where(i => i.BadgeNumber == BadgeNumber && i.IsApproved == null).Count();

            DateTime CurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var CurrentWeekMinutes = db.EnteredPeMinutes.Where(x => x.Timestamp >= CurrentWeek).Sum(x => x.Minutes);
            if (CurrentWeekMinutes == null)
            {
                ViewBag.CurrentWeekMinutes = 0;
            }
            else
            {
                ViewBag.CurrentWeekMinutes = CurrentWeekMinutes;
            }




            // Section for determining the minutes that can be used in a graph for the admin view. This will give the current month from 1 to today
            DateTime now = DateTime.Now;

            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month

            var AdminTrackMinutes = from MonthMinutes in db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp > lastDayLastMonth && x.Timestamp <= DateTime.Today)
                     let DatesMonthMinutes = MonthMinutes
                     group MonthMinutes by new { a = DatesMonthMinutes.Timestamp.Value.Day + "-" + DatesMonthMinutes.Timestamp.Value.Month + "-" + DatesMonthMinutes.Timestamp.Value.Year } into CompletedMinutes
                     select new
                     {
                         CompletedMinutesDateLabel = CompletedMinutes.Key, // This provides a list of dates that have minutes entered in for them
                         CompletedMinutesSum = CompletedMinutes.Sum(x=> x.Minutes) // This is a summation of all minutes put in for a particular day
                     };

            ViewBag.Dates = AdminTrackMinutes.Select(x => x.CompletedMinutesDateLabel.a).ToArray(); // x.CompletedMinutesDateLabe.a is needed because if we leave .a off it will give a result of "a":"Date"
            ViewBag.DTMinutes = AdminTrackMinutes.Select(x => x.CompletedMinutesSum).ToArray();
            // end of the data for the graph in admin view



            // this is a hard coded entry for the teachrs chart so that they can visually see if they missed a day. The other method that is used for admin view will skip the non entered days on the label and the teachers wont be able to see what they missed easily
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

            ViewBag.TwelveDays = TwelveDaysAgo.ToShortDateString();
            ViewBag.ElevenDays = ElevenDaysAgo.ToShortDateString();
            ViewBag.TenDays = TenDaysAgo.ToShortDateString();
            ViewBag.NineDays = NineDaysAgo.ToShortDateString();
            ViewBag.EightDays = EightDaysAgo.ToShortDateString();
            ViewBag.SevenDays = SevenDaysAgo.ToShortDateString();
            ViewBag.SixDays = SixDaysAgo.ToShortDateString();
            ViewBag.FiveDays = FiveDaysAgo.ToShortDateString();
            ViewBag.FourDays = FourDaysAgo.ToShortDateString();
            ViewBag.ThreeDays = ThreeDaysAgo.ToShortDateString();
            ViewBag.TwoDays = TwoDaysAgo.ToShortDateString();
            ViewBag.Yester = Yesterday.ToShortDateString();
            ViewBag.Now = Today.ToShortDateString();
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
            ViewBag.Today = db.EnteredPeMinutes.Where(x => x.BadgeNumber == BadgeNumber && x.Timestamp >= Today.Date).Sum(x => x.Minutes);
            // end of the sections where we build the chart data

            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == BadgeNumber).ToList();


            //DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
            //DateTime startingDate = DateTime.Today;

            //while (startingDate.DayOfWeek != weekStart)
            //    startingDate = startingDate.AddDays(-1);

            //DateTime previousWeekStart = startingDate.AddDays(-7);
            //DateTime previousWeekEnd = startingDate.AddDays(-1);




            return View(TeachersPeMinutes);
        }
        

        public ActionResult Manage()
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            DateTime ThirtyOneDays = DateTime.Now.AddDays(-31);
            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == BadgeNumber && i.Timestamp >= ThirtyOneDays).OrderByDescending(i => i.Timestamp).ToList();
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
                enteredPeMinute.IsApproved = 1;
                enteredPeMinute.ApprovedBy = TeacherNameVariable;
                enteredPeMinute.ApproveTime = DateTime.Now;


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


        // GET: Get Chart JSON Data
        public JsonResult GetMinutesGraph()
        {

            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int TeacherBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int

            DateTime TenDays = DateTime.Now.AddDays(-10);

            Array minutes = db.EnteredPeMinutes.Where(x => x.BadgeNumber == TeacherBadgeNumber).Select(x => x.Minutes).ToArray();

            return Json(minutes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDateGraph()
        {

            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int TeacherBadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int

            DateTime TenDays = DateTime.Now.AddDays(-10);



            Array dates = db.EnteredPeMinutes.Where(x => x.BadgeNumber == TeacherBadgeNumber).Select(x => x.Timestamp).ToArray();


            return Json(dates, JsonRequestBehavior.AllowGet);
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


