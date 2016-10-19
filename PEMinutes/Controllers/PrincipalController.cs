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
    public class PrincipalController : Controller
    {
        private PEMinutesEntities db = new PEMinutesEntities();
        private RenExtractEntities ren = new RenExtractEntities();

        // GET: Principal
        public ActionResult Index()
        {
            DateTime now = DateTime.Now;
            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month
            DateTime CurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var PrincipalView = db.EnteredPeMinutes.Where(x => x.Timestamp > lastDayLastMonth);






            // Section for determining the minutes that can be used in a graph for the admin view. This will give the current month from 1 to today
            //DateTime now = DateTime.Now;

            //DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            //lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month

            var AdminTrackMinutes = from MonthMinutes in db.EnteredPeMinutes.Where(x => x.Timestamp > CurrentWeek)
                                    let MonthTeacherNames = MonthMinutes
                                    group MonthMinutes by new { a = MonthTeacherNames.TeacherName} into CompletedMinutes
                                    select new
                                    {
                                        CompletedMinutesDateLabel = CompletedMinutes.Key, // This provides a list of dates that have minutes entered in for them
                                        CompletedMinutesSum = CompletedMinutes.Sum(x=>x.Minutes) // This is a summation of all minutes put in for a particular day
                                    };

            ViewBag.TeachersNames = AdminTrackMinutes.Select(x => x.CompletedMinutesDateLabel.a).ToArray(); // x.CompletedMinutesDateLabe.a is needed because if we leave .a off it will give a result of "a":"Date"
            ViewBag.TeachersMinutes = AdminTrackMinutes.Select(x => x.CompletedMinutesSum).ToArray();
            // end of the data for the graph in admin view












            return View(PrincipalView);
        }

        // GET: Principal/Details/5
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
























        // DISABLED 
        //// GET: Principal/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Principal/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] EnteredPeMinute enteredPeMinute)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.EnteredPeMinutes.Add(enteredPeMinute);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(enteredPeMinute);
        //}

        //// GET: Principal/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
        //    if (enteredPeMinute == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(enteredPeMinute);
        //}

        //// POST: Principal/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] EnteredPeMinute enteredPeMinute)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(enteredPeMinute).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(enteredPeMinute);
        //}

        //// GET: Principal/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
        //    if (enteredPeMinute == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(enteredPeMinute);
        //}

        //// POST: Principal/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    EnteredPeMinute enteredPeMinute = db.EnteredPeMinutes.Find(id);
        //    db.EnteredPeMinutes.Remove(enteredPeMinute);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
