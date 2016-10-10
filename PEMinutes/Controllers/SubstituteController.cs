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
            return View(schools);
        }

        // POST: Substitute/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime,IsLongTermSub")] SubMinute subMinute)
        {
            if (ModelState.IsValid)
            {
                db.SubMinutes.Add(subMinute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subMinute);
        }
        public ActionResult _GetTeachers(string schoolname)
        {
            List<SchoolTeachersWithADLogin> myTeacherList = ren.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == schoolname).OrderBy(i => i.TeacherLastName).ToList();
           
            return PartialView(myTeacherList);
        }

        public ActionResult _GetSubMinForm(string badge)
        {
            var teacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(x=>x.BADGE_NUM == badge);
            string[] schoolteacher = new string[4];
            schoolteacher[0] = teacher.TeacherLastName + ", " + teacher.TeacherFirstName;
            schoolteacher[1] = teacher.BADGE_NUM;
            schoolteacher[2] = teacher.Organization_Name;
            schoolteacher[3] = teacher.COURSE_TITLE;
            return PartialView();
        }
    }
}
