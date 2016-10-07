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
    public class SubstituteController : Controller
    {
        private PEMinutesEntities db = new PEMinutesEntities();

        // GET: Substitute
        public ActionResult Index()
        {
            return View(db.SubMinutes.ToList());
        }

        // GET: Substitute/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubMinute subMinute = db.SubMinutes.Find(id);
            if (subMinute == null)
            {
                return HttpNotFound();
            }
            return View(subMinute);
        }

        // GET: Substitute/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Substitute/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime,IsLongTermSub")] SubMinute subMinute)
        {
            if (ModelState.IsValid)
            {
                db.SubMinutes.Add(subMinute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subMinute);
        }

        // GET: Substitute/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubMinute subMinute = db.SubMinutes.Find(id);
            if (subMinute == null)
            {
                return HttpNotFound();
            }
            return View(subMinute);
        }

        // POST: Substitute/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime,IsLongTermSub")] SubMinute subMinute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subMinute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subMinute);
        }

        // GET: Substitute/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubMinute subMinute = db.SubMinutes.Find(id);
            if (subMinute == null)
            {
                return HttpNotFound();
            }
            return View(subMinute);
        }

        // POST: Substitute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubMinute subMinute = db.SubMinutes.Find(id);
            db.SubMinutes.Remove(subMinute);
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
