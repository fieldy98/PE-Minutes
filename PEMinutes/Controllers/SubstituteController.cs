using System;
using System.Linq;
using System.Web.Mvc;
using PEMinutes.EF;
using PEMinutes.Models;
using PEMinutes.ViewModels;

namespace PEMinutes.Controllers
{
    public class SubstituteController : Controller
    {
        private readonly PEMinutesEntities _db = new PEMinutesEntities();
        private readonly RenExtractEntities _ren = new RenExtractEntities();

        public ActionResult Index()
        {
            var schools = _ren.PEMinutesTeacherCounts.Where(x=>x.Organization_Name.Contains("Elemen")); // Limiting the dropdown to only elementary schools
            var svm = new SubstituteViewModel();
            foreach (var item in schools)
            {
                var sl = new SchList {SchoolName = item.Organization_Name};
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
        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,InstructionTime,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] SubMinute sub, SimplerAesModel encryptedBadge, string selectedbadge)
        {
            if (!ModelState.IsValid) return View(sub);
            var decryptedBadge = encryptedBadge.Decrypt(selectedbadge);
            var badgeNumber = int.Parse(decryptedBadge);  // convert string to int
            var selectedTeacher = _ren.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == decryptedBadge); //Finding the teacher that matches the selected badge number

            // Build variable with information not gathered from user.
            sub.TeacherName = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            sub.School = selectedTeacher.Organization_Name;
            sub.Grade = selectedTeacher.COURSE_TITLE;
            sub.BadgeNumber = badgeNumber;
            sub.Timestamp = DateTime.Now;

            // Apply the modifications and then save to the database
            _db.SubMinutes.Add(sub);
            _db.SaveChanges();
            return RedirectToAction("Index", "Authentication"); // takes the user back to the login page to choose subs or teacher
        }
        // This loads the _GetTeachers Partial
        public ActionResult _GetTeachers(SimplerAesModel decryptedBadge, string selectedSchool)
        {
            var svm = new SubstituteViewModel();
            // This is the query for selecting teachers from the selected school and they are put into a list for the dropdown
            var selectedTeachers = _ren.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == selectedSchool && i.COURSE_TITLE.Contains("PS") == false && i.COURSE_TITLE.Contains("Kind") == false).OrderBy(i => i.TeacherLastName).ToList();
            foreach (var item in selectedTeachers)
            {
                var tl = new TeachList
                {
                    SchoolName = item.Organization_Name,
                    TeacherName = item.TeacherLastName + ", " + item.TeacherFirstName,
                    BadgeNumber = decryptedBadge.Encrypt(item.BADGE_NUM)
                };
                svm.TeacherList.Add(tl);
            }
            svm.TeacherList = svm.TeacherList.ToList();
            return PartialView(svm);
        }
    }
}
