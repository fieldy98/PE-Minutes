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
