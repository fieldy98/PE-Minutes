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
        public ActionResult Index([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime,IsLongTermSub")] SubMinute subMinute, string badge)
        {
            if (ModelState.IsValid)
            {
                int BadgeNumber = Int32.Parse(badge);  // convert string to int
                SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == badge );

                // Build variable with information not gathered from user.
                subMinute.TeacherName = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
                subMinute.School = SelectedTeacher.Organization_Name;
                subMinute.Grade = SelectedTeacher.COURSE_TITLE;
                subMinute.BadgeNumber = BadgeNumber;
                subMinute.Timestamp = DateTime.Now;

                // Apply the modifications and then save to the database
                db.SubMinutes.Add(subMinute);
                db.SaveChanges();
                return RedirectToAction("Index");




                //db.SubMinutes.Add(subMinute);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            return View(subMinute);
        }
        public ActionResult _GetTeachers(string schoolname)
        {
            List<SchoolTeachersWithADLogin> myTeacherList = ren.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == schoolname).OrderBy(i => i.TeacherLastName).ToList();
           
            return PartialView(myTeacherList);
        }

        public ActionResult _GetSubMinForm()
        {
            return PartialView();
        }
    }
}
