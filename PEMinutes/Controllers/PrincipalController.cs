using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using PEMinutes.ViewModels;
using System.Web.Mvc;
using PEMinutes.EF;

namespace PEMinutes.Controllers
{
    public class PrincipalController : Controller
    {
        private PEMinutesEntities db = new PEMinutesEntities();
        private RenExtractEntities ren = new RenExtractEntities();



        // GET: /Principal/Index
        public ActionResult Index()
        {
            DateTime now = DateTime.Now;
            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month
            DateTime CurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var lastweek = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
            ViewBag.lastweek = lastweek;  // used to find falling behind teachers
            ViewBag.CurrentWeek = CurrentWeek; // used to find falling behind teachers
            
            var EnteredBadgeString = User.Identity.Name;
            SchoolToPrincipal SelectedPrincipal = ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            var SelectedSchool = SelectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = SelectedPrincipal.Principal;

            var PrincipalView = db.EnteredPeMinutes.Where(x => x.InstructionTime > lastDayLastMonth && x.School == SelectedSchool ).OrderBy(x=>x.TeacherName); // select all minutes from the school the principal belongs to
            // Graph tracking minutes per teacher
            var AdminTrackMinutes = from MonthMinutes in db.EnteredPeMinutes.Where(x => x.InstructionTime > CurrentWeek && x.School == SelectedSchool).OrderByDescending(x=>x.Minutes)
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

        // GET: /Principal/Reports
        public ActionResult Reports()
        {
            var EnteredBadgeString = User.Identity.Name;
            SchoolToPrincipal SelectedPrincipal = ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            var SelectedSchool = SelectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = SelectedPrincipal.Principal;

            DateTime PastTenDays = DateTime.Now.AddDays(-11);

            var SchoolReport = db.EnteredPeMinutes.Where( x => x.School == SelectedSchool && x.InstructionTime > PastTenDays).OrderBy(x => x.Minutes); // select all minutes from the school the principal belongs to

            return View(SchoolReport);
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
        //public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,InstructionTime,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] EnteredPeMinute enteredPeMinute)
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
        //public ActionResult Edit([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,InstructionTime,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] EnteredPeMinute enteredPeMinute)
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
