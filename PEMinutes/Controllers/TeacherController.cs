using System;
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
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }
    public class TeacherController : Controller
    {
        private readonly RenExtractEntities _ren = new RenExtractEntities();
        private readonly PEMinutesEntities _db = new PEMinutesEntities();

        // GET: Teacher
        public ActionResult Index()
        {
            var enteredBadgeString = User.Identity.Name; 
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var badgeNumber = int.Parse(enteredBadgeString);  // convert string to int
            ViewBag.Name = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            ViewBag.NeedsApproval = _db.SubMinutes.Count(i => i.BadgeNumber == badgeNumber && i.IsApproved == null);  // tells teacher how many entries need approval
            var now = DateTime.Now.AddDays(1);
            var end = DateTime.Now.AddDays(-13);
            var tivm = new TeacherIndexViewModel();
            var teachersPeMinutes = _db.EnteredPeMinutes.Where(i => i.BadgeNumber == badgeNumber && i.InstructionTime >= end.Date && i.InstructionTime < now.Date).OrderBy(x=>x.InstructionTime).ToList(); // Finds all of the teachers minutes for the last 2 weeks

            foreach(var item in teachersPeMinutes)
            {
                MinuteCount mc = new MinuteCount();

                mc.Minutes = item.Minutes;
                mc.Date = item.InstructionTime.Value.ToShortDateString();
                mc.ID = item.ID;
                mc.Activity = item.Activity;
                tivm.MinCount.Add(mc);
            }

            tivm.Percentage = (float)(teachersPeMinutes.Sum(x => x.Minutes) / 2);

            tivm.Minutes = teachersPeMinutes.Sum(x=>x.Minutes);
            tivm.MinCount = tivm.MinCount.ToList();
            return View(tivm);
        }

        // GET: Teacher/Manage
        public ActionResult Manage()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var thisMonth = DateTime.Today.ToString("MMMM");
            ViewBag.ThisMonth = thisMonth;
            var enteredBadgeString = User.Identity.Name;
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var enteredBadgeNumber = int.Parse(enteredBadgeString);  // convert string to int
            ViewBag.Name = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;

            var teachersPeMinutes = _db.EnteredPeMinutes.Where(i => i.BadgeNumber == enteredBadgeNumber && i.InstructionTime > thirtyDaysAgo).OrderByDescending(i => i.InstructionTime).ToList(); // Finds the minutes for the signed in teacher for the current month
            ViewBag.CurrentMonthMinuteCount = teachersPeMinutes.Count();
            ViewBag.TotalMonthMins = teachersPeMinutes.Sum(x => x.Minutes);
            return View(teachersPeMinutes);
        }

        // GET: Teacher/Details/5
        public ActionResult Details(int? id)
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
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
            if (!ModelState.IsValid) return View(enteredPeMinute);
            var enteredBadgeString = User.Identity.Name;
            var badgeNumber = int.Parse(enteredBadgeString);  // convert string to int

            // Associate Badge => Staff
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);

            // Build variable with information not gathered from user.
            var teacherNameVariable = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            enteredPeMinute.TeacherName = teacherNameVariable;
            enteredPeMinute.School      = selectedTeacher.Organization_Name;
            enteredPeMinute.Grade       = selectedTeacher.COURSE_TITLE;
            enteredPeMinute.BadgeNumber = badgeNumber;
            enteredPeMinute.Timestamp   = DateTime.Now;

            // Apply the modifications and then save to the database
            _db.EnteredPeMinutes.Add(enteredPeMinute);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Teacher/Approve
        public ActionResult Approve()
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var teacherBadgeNumber = int.Parse(enteredBadgeString);  // convert string to int
            ViewBag.Name = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            ViewBag.School = selectedTeacher.Organization_Name;

            // Get Teachers minutes that need approval from the SubMinutes Table.
            var subMinutesForApproval = _db.SubMinutes.Where(i => i.BadgeNumber == teacherBadgeNumber && i.IsApproved == null).OrderByDescending(i => i.InstructionTime).ToList();
            return View(subMinutesForApproval);
        }

        // POST: Approve
        [HttpPost]
        public ActionResult Approve(int selectedId)
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var teacherNameVariable = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            var minute = _db.SubMinutes.FirstOrDefault(x => x.ID == selectedId);
            minute.IsApproved = 1;
            minute.ApprovedBy = teacherNameVariable;
            minute.ApproveTime = DateTime.Now;
            _db.Entry(minute).State = EntityState.Modified;
            _db.SaveChanges();

            return Json(new { success = true });
        }

        // POST: MoveSubToTeacher
        [HttpPost]
        public ActionResult MoveSubToTeacher(string selectedSubstituteName, int selectedMinutes, string selectedActivity, DateTime selectedInstructionTime, EnteredPeMinute enteredPeMinute)
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var teacherNameVariable = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            enteredPeMinute.TeacherName = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            enteredPeMinute.Minutes = selectedMinutes;
            enteredPeMinute.Activity = selectedActivity;
            enteredPeMinute.Timestamp = DateTime.Now;
            enteredPeMinute.InstructionTime = selectedInstructionTime;
            enteredPeMinute.SubstituteName = selectedSubstituteName;
            var badgeNumber = int.Parse(selectedTeacher.BADGE_NUM);  // convert string to int
            enteredPeMinute.BadgeNumber = badgeNumber;
            enteredPeMinute.School = selectedTeacher.Organization_Name;
            enteredPeMinute.Grade = selectedTeacher.COURSE_TITLE;
            enteredPeMinute.IsApproved = 1;
            enteredPeMinute.ApprovedBy = teacherNameVariable;
            enteredPeMinute.ApproveTime = DateTime.Now;
            _db.EnteredPeMinutes.Add(enteredPeMinute);
            _db.SaveChanges();
            return Json(new { success = true });
        }

        // GET: Teacher/Edit/5
        // Edit Minutes
        public ActionResult Edit(int? id)
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            ViewBag.Name = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enteredPeMinute = _db.EnteredPeMinutes.Find(id);
            if (enteredPeMinute == null)
            {
                return HttpNotFound();
            }
            var tevm = new TeacherEditViewModel                     // Building the list 
            {
                Id = enteredPeMinute.ID,
                Minutes = enteredPeMinute.Minutes,
                InstructionTime = enteredPeMinute.InstructionTime,
                Activity = enteredPeMinute.Activity,
                TeacherName = enteredPeMinute.TeacherName,
                SubstituteName = enteredPeMinute.SubstituteName,
                ApprovedBy = enteredPeMinute.ApprovedBy,
                BadgeNumber = enteredPeMinute.BadgeNumber,
                Grade = enteredPeMinute.Grade,
                IsApproved = enteredPeMinute.IsApproved,
                School = enteredPeMinute.School,
                Timestamp = enteredPeMinute.Timestamp,
                ApprovedTime = enteredPeMinute.ApproveTime
            };
            return View(tevm);
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime,InstructionTime")] EnteredPeMinute enteredPeMinute)
        {
            if (!ModelState.IsValid) return View(enteredPeMinute);
            enteredPeMinute.IsApproved = 1;                             // If the minute is entered by the teacher it does not need approval, so set 1.
            enteredPeMinute.ApprovedBy = enteredPeMinute.TeacherName;
            enteredPeMinute.ApproveTime = DateTime.Now;
            _db.Entry(enteredPeMinute).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Teacher/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enteredPeMinute = _db.EnteredPeMinutes.Find(id);      // Find the record to delete by its id
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
            var enteredPeMinute = _db.EnteredPeMinutes.Find(id);
            _db.EnteredPeMinutes.Remove(enteredPeMinute);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

