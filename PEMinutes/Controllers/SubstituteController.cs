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
        private RenExtractEntities ren = new RenExtractEntities();

        // GET: Substitute
        public ActionResult Index()
        {
            var schools = ren.SchoolTeachersWithADLogins;
            IEnumerable<SelectListItem> items = schools.OrderBy(x => x.Organization_Name).Select(x => new SelectListItem
            {
                Value = x.Organization_Name,
                Text = x.Organization_Name
            }).Distinct();
            IEnumerable<SelectListItem> teachers = schools.OrderBy(x => x.TeacherLastName).Select(x => new SelectListItem
            {
                Value = x.BADGE_NUM,
                Text = x.TeacherLastName + ", " + x.TeacherFirstName
            });
            ViewBag.School = items;
            ViewBag.Teacher = teachers;

            return View(schools);
        }

        // GET: Substitute/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Substitute/Create
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
    }
}
