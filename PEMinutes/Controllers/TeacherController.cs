using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PEMinutes.EF;

namespace PEMinutes.Controllers
{
    public class TeacherController : Controller
    {
        private RenExtractEntities ren = new RenExtractEntities();
        private PEMinutesEntities db = new PEMinutesEntities();

        // GET: Teacher
        // Landing Page
        public ActionResult Index(string Badge)
        {
            var teacher = from s in ren.SchoolTeachersWithADLogins select s;

            if (!String.IsNullOrEmpty(Badge))
            {
                teacher = teacher.Where(s => s.BADGE_NUM == Badge);
                foreach (var item in teacher)
                {
                    ViewBag.Teacher = item.LOGIN_NAME;
                    ViewBag.FirstName = item.TeacherFirstName;
                    ViewBag.LastName = item.TeacherLastName;
                    ViewBag.Badge = item.BADGE_NUM;
                    ViewBag.School = item.Organization_Name;
                }
                return View(teacher);
            }


            return View(teacher);
        }


        public JsonResult IdentifyTeacher(int EnteredBadge)
        {
            EnteredPeMinute SelectedTeacher = db.EnteredPeMinutes.FirstOrDefault();


            return Json(SelectedTeacher, JsonRequestBehavior.AllowGet);
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
                db.EnteredPeMinutes.Add(enteredPeMinute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(enteredPeMinute);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
