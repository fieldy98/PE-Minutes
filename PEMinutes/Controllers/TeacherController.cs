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

//
// OVERVIEW:
//      This controller handles functions for the Teacher role. 
//      They are allowed to Create, View and Edit their minutes.
//      They are also able to approve minutes submitted by their Sub.
//

namespace PEMinutes.Controllers
{
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
            List<EnteredPeMinute> TeachersPeMinutes = db.EnteredPeMinutes.Where(i => i.BadgeNumber == BadgeNumber).OrderByDescending(i => i.Timestamp).ToList();
            
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

        // POST: Checkout 
        public ActionResult Approve(int IssuedApproval)
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            var TeacherNameVariable = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
            var Minute = db.SubMinutes.FirstOrDefault(x => x.ID == IssuedApproval);

            Minute.IsApproved = 1;
            Minute.ApprovedBy = TeacherNameVariable;
            Minute.ApproveTime = DateTime.Now;
            db.SubMinutes.Add(Minute);
            db.SaveChanges();

            //EnteredPeMinute AddApprovedMinute = new EnteredPeMinute()
            //{
               
            //};

            //db.EnteredPeMinutes.Attach();
            //db.SaveChanges();


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


