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

        public ActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        
        public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] SubMinute sub, string selectedbadge)
        {
            if (ModelState.IsValid)
            {
                int BadgeNumber = Int32.Parse(selectedbadge);  // convert string to int
                SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == selectedbadge);

                // Build variable with information not gathered from user.
                sub.TeacherName = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
                sub.School = SelectedTeacher.Organization_Name;
                sub.Grade = SelectedTeacher.COURSE_TITLE;
                sub.BadgeNumber = BadgeNumber;
                sub.Timestamp = DateTime.Now;

                // Apply the modifications and then save to the database
                db.SubMinutes.Add(sub);
                db.SaveChanges();
                return RedirectToAction("Index", "Authentication");
            }
            return View(sub);
        }

        public ActionResult _GetTeachers(string schoolname)
        {
            List<SchoolTeachersWithADLogin> myTeacherList = ren.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == schoolname && i.COURSE_TITLE.Contains("PS") == false && i.COURSE_TITLE.Contains("Kind") == false).OrderBy(i => i.TeacherLastName).ToList();
           
            return PartialView(myTeacherList);
        }
    }
}
