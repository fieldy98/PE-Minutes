using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using PEMinutes.EF;
using PEMinutes.Models;
using PEMinutes.ViewModels;

namespace PEMinutes.Controllers
{
    public class SubstituteController : Controller
    {
        private PEMinutesEntities db = new PEMinutesEntities();
        private RenExtractEntities ren = new RenExtractEntities();

        public ActionResult Index()
        {
            var schools = ren.PEMinutesTeacherCounts.Where(x=>x.Organization_Name.Contains("Elemen")); // Limiting the dropdown to only elementary schools
            SubstituteViewModel svm = new SubstituteViewModel();
            List<SchList> SchoolList = new List<SchList>();
            foreach (var item in schools)
            {
                SchList sl = new SchList();
                sl.SchoolName = item.Organization_Name;
                svm.SchoolList.Add(sl);
            }
            svm.SchoolList = svm.SchoolList.ToList();
            return View(svm);
        }

        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Substitute/Create
        // This is how the SubMinutes are created
        [HttpPost]
        
        public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] SubMinute sub, SimplerAESModel EncryptedBadge, string selectedbadge)
        {
            if (ModelState.IsValid)
            {


                string DecryptedBadge = EncryptedBadge.Decrypt(selectedbadge);
                int BadgeNumber = Int32.Parse(DecryptedBadge);  // convert string to int
                SchoolTeachersWithADLogin SelectedTeacher = ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == DecryptedBadge); //Finding the teacher that matches the selected badge number

                // Build variable with information not gathered from user.
                sub.TeacherName = SelectedTeacher.TeacherFirstName + " " + SelectedTeacher.TeacherLastName;
                sub.School = SelectedTeacher.Organization_Name;
                sub.Grade = SelectedTeacher.COURSE_TITLE;
                sub.BadgeNumber = BadgeNumber;
                sub.Timestamp = DateTime.Now;

                // Apply the modifications and then save to the database
                db.SubMinutes.Add(sub);
                db.SaveChanges();
                return RedirectToAction("Index", "Authentication"); // takes the user back to the login page to choose subs or teacher
            }
            return View(sub);
        }
        // This loads the _GetTeachers Partial
        public ActionResult _GetTeachers(SimplerAESModel DecryptedBadge, string SelectedSchool)
        {
            SubstituteViewModel svm = new SubstituteViewModel();
            // This is the query for selecting teachers from the selected school and they are put into a list for the dropdown
            List<SchoolTeachersWithADLogin> SelectedTeachers = ren.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == SelectedSchool && i.COURSE_TITLE.Contains("PS") == false && i.COURSE_TITLE.Contains("Kind") == false).OrderBy(i => i.TeacherLastName).ToList();
            List<TeachList> TeacherList = new List<TeachList>();
            foreach (var item in SelectedTeachers)
            {
                TeachList tl = new TeachList();
                tl.SchoolName = item.Organization_Name;
                tl.TeacherName = item.TeacherLastName + ", " + item.TeacherFirstName;
                tl.BadgeNumber = DecryptedBadge.Encrypt(item.BADGE_NUM);
                svm.TeacherList.Add(tl);
            }
            svm.TeacherList = svm.TeacherList.ToList();
            return PartialView(svm);
        }
    }
}
