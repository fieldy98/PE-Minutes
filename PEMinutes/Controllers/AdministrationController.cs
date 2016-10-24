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
    public class AdministrationController : Controller
    {
        private PEMinutesEntities db = new PEMinutesEntities();
        private RenExtractEntities ren = new RenExtractEntities();

        // GET: Administration
        public ActionResult Index(string SelectedDate)
        {
            DateTime StartDay = DateTime.Now;
            if(!String.IsNullOrEmpty(SelectedDate))
            {
                DateTime Date = Convert.ToDateTime(SelectedDate);
                StartDay = Date.Date;
            }
            
            DateTime lastDayLastMonth = new DateTime(StartDay.Year, StartDay.Month, 1);
            lastDayLastMonth = lastDayLastMonth.AddDays(-1);  // selecting last month because I want to make sure that it is everything in the current month
            DateTime CurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var lastweek = StartDay.AddDays(-14);
            ViewBag.lastweek = lastweek;  // used to find falling behind teachers
            ViewBag.CurrentWeek = CurrentWeek; // used to find falling behind teachers

            var EnteredBadgeString = User.Identity.Name;
            MinutesAdmin SelectedAdmin = ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedAdmin.FIRST_NAME + " " + SelectedAdmin.LAST_NAME; ;

            var AdminView = db.EnteredPeMinutes.Where(x => x.InstructionTime >= lastweek.Date && x.InstructionTime < StartDay.Date && x.School.Contains("Elem")).OrderBy(x => x.School); // select all minutes from the school the principal belongs to

            AdministrationViewModel avm = new AdministrationViewModel();
            PEMinutesTeacherCount ptc = new PEMinutesTeacherCount();
            EnteredPeMinute epm = new EnteredPeMinute();

            List<TeacherCount> TeachCount = new List<TeacherCount>();
            List<PEMinutesTeacherCount> NumberTeachers = ren.PEMinutesTeacherCounts.Where(x=>x.Organization_Name.Contains("Elem")).ToList();

            foreach (var item in NumberTeachers)
            {
                TeacherCount tc = new TeacherCount();
                var schoolfullname = item.Organization_Name;
                tc.CountTeacher = item.TEACHER;
                String schoolname = schoolfullname.Substring(0, schoolfullname.Length - 18);
                tc.SchoolName = schoolname;
                var count = 0;
                foreach (var teach in AdminView.Where(x => x.School == item.Organization_Name).GroupBy(x => x.TeacherName))
                {
                    var sum = teach.Sum(x => x.Minutes);
                    if (sum >= 100)
                    {
                        count++;

                    }
                }
                tc.MeetReq = count;


                tc.Percent = ((float)count / item.TEACHER) *100;
                avm.TeachCount.Add(tc);
            }
            avm.Date = DateTime.Now.ToShortDateString();
            avm.TeachCount = avm.TeachCount.ToList();
            return View(avm);

        }

        [HttpGet]
        public ActionResult TotalGraph(DateTime SelectedDate)
        {
            if (SelectedDate != null)
            {
                DateTime date = SelectedDate.AddDays(-1);
                var SchoolTotals = db.EnteredPeMinutes.Where(x => x.InstructionTime == SelectedDate).GroupBy(x => x.School);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Reports(int timeFrame, int divisor)
        {
            DateTime now = DateTime.Now;
            DateTime SelectedTimeFrame = now.AddDays(timeFrame);
            DateTime CurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // Making each new week start on Monday.
            var lastweek = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
            ViewBag.lastweek = lastweek;  // used to find falling behind teachers
            ViewBag.CurrentWeek = CurrentWeek; // used to find falling behind teachers

            ViewBag.Divisor = divisor;

            var EnteredBadgeString = User.Identity.Name;
            MinutesAdmin SelectedAdmin = ren.MinutesAdmins.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            ViewBag.Name = SelectedAdmin.FIRST_NAME + " " + SelectedAdmin.LAST_NAME; ;

            var AdminView = db.EnteredPeMinutes.Where(x => x.InstructionTime >= SelectedTimeFrame.Date && x.InstructionTime < now.Date).OrderBy(x => x.School); // select all minutes from the school the principal belongs to
            return View(AdminView);
        }

        // GET: Administration/Details/5
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

        // GET: Administration/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Administration/Edit/5
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

        // POST: Administration/Edit/5
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

        // GET: Administration/Delete/5
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

        // POST: Administration/Delete/5
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
