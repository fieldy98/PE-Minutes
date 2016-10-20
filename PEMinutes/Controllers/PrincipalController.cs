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

            var PrincipalView = db.EnteredPeMinutes.Where(x => x.Timestamp > lastDayLastMonth && x.School == SelectedSchool ).OrderBy(x=>x.TeacherName); // select all minutes from the school the principal belongs to
            // Graph tracking minutes per teacher
            var AdminTrackMinutes = from MonthMinutes in db.EnteredPeMinutes.Where(x => x.Timestamp > CurrentWeek && x.School == SelectedSchool).OrderByDescending(x=>x.Minutes)
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
        public ActionResult Reports(string _dateRange, string _teacher)
        {
            PrincipalViewModel pvm = new PrincipalViewModel();
            var EnteredBadgeString = User.Identity.Name;
            SchoolToPrincipal SelectedPrincipal = ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            var SelectedSchool = SelectedPrincipal.ORGANIZATION_NAME;

            List<SchoolTeachersWithADLogin> myfellowteachers = ren.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == SelectedSchool
                && i.COURSE_TITLE.Contains("PS") == false && i.COURSE_TITLE.Contains("Kind") == false).ToList();
            //foreach (var item in myfellowteachers)
            //{
            //    tsl.TeacherName = item.TeacherFirstName + " " + item.TeacherLastName;

            //    pvm.SchoolTeachers = tsl;
            //}
            //pvm.SchoolTeachers = pvm.SchoolTeachers.ToList();
            





            if (!String.IsNullOrEmpty(_dateRange))
            {
                
                var dates = _dateRange.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var start = dates[0];                               // Select first and second sections
                var end = dates[1];
                DateTime StartDate = Convert.ToDateTime(start);         // Convert 'start' and 'end' to DateTime
                DateTime EndDate = Convert.ToDateTime(end).AddDays(1);

                if (_dateRange != null && _teacher != null)
                {
                    var SearchDateAndTeacher = db.EnteredPeMinutes.Where(x => x.School == SelectedSchool && x.TeacherName == _teacher && x.Timestamp >= StartDate && x.Timestamp <= EndDate);
                    return View(SearchDateAndTeacher);
                }
                else if (_dateRange != null)
                {
                    var SearchDate = db.EnteredPeMinutes.Where(x => x.School == SelectedSchool && x.Timestamp >= StartDate && x.Timestamp <= EndDate);
                    return View(SearchDate);
                }

            }
            else if (!String.IsNullOrEmpty(_teacher))
            {
                var SearchTeacher = db.EnteredPeMinutes.Where(x => x.School == SelectedSchool && x.TeacherName.Contains(_teacher));
                return View(SearchTeacher);
            }
            var AllMinutesAtSite = db.EnteredPeMinutes.Where(x => x.School == SelectedSchool);
            return View(pvm);
            
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
