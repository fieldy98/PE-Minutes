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
            DateTime lastweek = DateTime.Now.AddDays(-14);
            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month, 1);
            
            var EnteredBadgeString = User.Identity.Name;
            SchoolToPrincipal SelectedPrincipal = ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == EnteredBadgeString);
            int BadgeNumber = Int32.Parse(EnteredBadgeString);  // convert string to int
            var SelectedSchool = SelectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = SelectedPrincipal.Principal;

            var PrincipalView = db.EnteredPeMinutes.Where(x => x.InstructionTime > lastweek && x.School == SelectedSchool ).Select(x=>x.TeacherName).Distinct(); // select all minutes from the school the principal belongs to

            PrincipalIndexViewModel pivm = new PrincipalIndexViewModel();
            EnteredPeMinute epm = new EnteredPeMinute();

            List<MeetingReq> MeetReq = new List<MeetingReq>();
            List<NotMeetingReq> NotReq = new List<NotMeetingReq>();
            List<Graphing> Graph = new List<Graphing>();

            foreach (var item in PrincipalView)
            {
                MeetingReq mr = new MeetingReq();
                
                var sumMinutes = db.EnteredPeMinutes.Where(x => x.InstructionTime > lastweek && x.TeacherName == item).Sum(x => x.Minutes);
                if(sumMinutes >= 200)
                {
                    mr.TeacherName = item;
                    mr.Minutes = sumMinutes;
                    pivm.MeetReq.Add(mr);
                }
                    
                
            }

            foreach (var item in PrincipalView)
            {
                NotMeetingReq nmr = new NotMeetingReq();

                var sumMinutes = db.EnteredPeMinutes.Where(x => x.InstructionTime > lastweek && x.TeacherName == item).Sum(x => x.Minutes);
                if (sumMinutes < 200)
                {
                    nmr.TeacherName = item;
                    nmr.Minutes = sumMinutes;
                    pivm.NotReq.Add(nmr);
                }

                
            }

            foreach (var item in PrincipalView)
            {
                Graphing g = new Graphing();

                var sumMinutes = db.EnteredPeMinutes.Where(x => x.InstructionTime > lastweek && x.TeacherName == item).Sum(x => x.Minutes);
                    g.TeacherName = item;
                    g.Minutes = sumMinutes;
                    pivm.Graph.Add(g);
            }
            pivm.MeetReq = pivm.MeetReq.ToList();
            pivm.NotReq = pivm.NotReq.ToList();
            pivm.Graph = pivm.Graph.ToList();
            // end of the data for the graph in admin view

            return View(pivm);
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
