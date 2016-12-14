using System;
using System.Linq;
using System.Net;
using PEMinutes.ViewModels;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PEMinutes.EF;

namespace PEMinutes.Controllers
{
    public class PrincipalController : Controller
    {
        private readonly PEMinutesEntities _db = new PEMinutesEntities();
        private readonly RenExtractEntities _ren = new RenExtractEntities();

        // GET: /Principal/Index
        public ActionResult Index(string selectedDate)
        {
            var enteredBadgeString = User.Identity.Name;
            var selectedPrincipal = _ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var selectedSchool = selectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = selectedPrincipal.Principal;

            var now = _db.EnteredPeMinutes.Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).FirstOrDefault().Value.Date;
            if (!string.IsNullOrEmpty(selectedDate))
            {
                var date = Convert.ToDateTime(selectedDate);
                now = date.Date;
            }
            var TenEntryDaysBack = _db.EnteredPeMinutes.Where(x => x.InstructionTime <= now).Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).Take(10).LastOrDefault().Value.Date;

            var principalView = _db.EnteredPeMinutes.Where(x => x.InstructionTime > TenEntryDaysBack && x.InstructionTime <= now && x.School == selectedSchool).Select(x => x.TeacherName).Distinct(); // select all minutes from the school the principal belongs to
            var pivm = new PrincipalIndexViewModel();

            foreach (var item in principalView)
            {
                var mr = new MeetingReq();
                var sumMinutes = _db.EnteredPeMinutes.Where(x => x.InstructionTime > TenEntryDaysBack && x.InstructionTime <= now && x.TeacherName == item).Sum(x => x.Minutes);
                if (!(sumMinutes >= 200)) continue;
                mr.TeacherName = item;
                mr.Minutes = sumMinutes;
                pivm.MeetReq.Add(mr);
            }

            foreach (var item in principalView)
            {
                var nmr = new NotMeetingReq();

                var sumMinutes = _db.EnteredPeMinutes.Where(x => x.InstructionTime > TenEntryDaysBack && x.InstructionTime <= now && x.TeacherName == item).Sum(x => x.Minutes);
                if (!(sumMinutes < 200)) continue;
                nmr.TeacherName = item;
                nmr.Minutes = sumMinutes;
                pivm.NotReq.Add(nmr);
            }

            foreach (var item in principalView)
            {
                var g = new Graphing();
                var sumMinutes = _db.EnteredPeMinutes.Where(x => x.InstructionTime > TenEntryDaysBack && x.InstructionTime <= now && x.TeacherName == item).Sum(x => x.Minutes);
                g.TeacherName = item;
                g.Minutes = sumMinutes;
                pivm.Graph.Add(g);
            }

            pivm.Date = now.ToShortDateString();
            pivm.DateStart = TenEntryDaysBack.ToShortDateString();
            pivm.DateEnd = now.ToShortDateString();
            pivm.MeetReq = pivm.MeetReq.ToList();
            pivm.NotReq = pivm.NotReq.ToList();
            pivm.Graph = pivm.Graph.ToList();
            // end of the data for the graph in admin view
            return View(pivm);
        }

        // GET: /Principal/Reports
        public ActionResult Reports()
        {
            PrincipalIndexViewModel pivm = new PrincipalIndexViewModel();

            var enteredBadgeString = User.Identity.Name;
            var selectedPrincipal = _ren.SchoolToPrincipals.FirstOrDefault(i => i.BADGE_NUM == enteredBadgeString);
            var selectedSchool = selectedPrincipal.ORGANIZATION_NAME;
            ViewBag.Name = selectedPrincipal.Principal;
            var startDay = _db.EnteredPeMinutes.Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).FirstOrDefault().Value.Date;
            var pastTenDays = _db.EnteredPeMinutes.Where(x => x.InstructionTime <= startDay).Select(x => x.InstructionTime).DistinctBy(x => x.Value.Date).OrderByDescending(x => x).Take(10).LastOrDefault().Value.Date;
            var schoolReport = _db.EnteredPeMinutes.Where(x => x.School == selectedSchool && x.InstructionTime > pastTenDays).OrderBy(x => x.Minutes); // select all minutes from the school the principal belongs to

            foreach (var item in schoolReport)
            {
                PrinicipalReports pr = new PrinicipalReports();

                pr.TeacherName = item.TeacherName;
                pr.Minutes = item.Minutes;
                var time = Convert.ToDateTime(item.InstructionTime);
                pr.InstructionTime = time.ToShortDateString();

                pivm.ListReports.Add(pr);
            }

            pivm.ListReports = pivm.ListReports.ToList();

            return View(pivm);
        }

        // GET: Principal/Details/5
        public ActionResult Details(int? id)
        {
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
    }
}
