using System;
using System.Linq;
using System.Web.Mvc;
using PEMinutes.EF;
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
        public ActionResult _identifyStaff(string selectedbadge)
        {
            
            var svm = new SubstituteViewModel();
            var selectedTeacher = _db.SchoolTeachersWithADLogins.FirstOrDefault(x => x.BADGE_NUM == selectedbadge);
            svm.BadgeNumber = selectedTeacher.BADGE_NUM;
            svm.SchoolName = selectedTeacher.Organization_Name;
            return PartialView("partials/_create", svm);
        }
        // POST: Substitute/Create
        [HttpPost]
        public ActionResult _identifyStaff([Bind(Include = "ID,TeacherName,Minutes,BadgeNumber,School,Grade,Activity,InstructionTime,Timestamp,SubstituteName,IsApproved,ApprovedBy,ApproveTime")] SubMinute sub, string selectedbadge)
        {
            if (!ModelState.IsValid) return View(sub);
            var selectedTeacher = _db.SchoolTeachersWithADLogins.FirstOrDefault(i => i.BADGE_NUM == selectedbadge); //Finding the teacher that matches the selected badge number

            // Build variable with information not gathered from user.
            sub.TeacherName = selectedTeacher.TeacherFirstName + " " + selectedTeacher.TeacherLastName;
            sub.School = selectedTeacher.Organization_Name;
            sub.Grade = selectedTeacher.COURSE_TITLE;
            sub.BadgeNumber = int.Parse(selectedbadge);
            sub.Timestamp = DateTime.Now;

            // Apply the modifications and then save to the database
            _db.SubMinutes.Add(sub);
            _db.SaveChanges();
            return RedirectToAction("Index", "Authentication"); // takes the user back to the login page to choose subs or teacher
        }
        public ActionResult _GetTeachers(string selectedSchool)
        {
            var svm = new SubstituteViewModel();
            var selectedTeachers = _db.SchoolTeachersWithADLogins.Where(i => i.Organization_Name == selectedSchool && i.COURSE_TITLE.Contains("PS") == false && i.COURSE_TITLE.Contains("Kind") == false).OrderBy(i => i.TeacherLastName).ToList();
            foreach (var item in selectedTeachers)
            {
                var tl = new TeachList
                {
                    SchoolName = item.Organization_Name,
                    TeacherName = item.TeacherLastName + ", " + item.TeacherFirstName,
                    BadgeNumber = item.BADGE_NUM
                };
                svm.TeacherList.Add(tl);
            }
            svm.TeacherList = svm.TeacherList.ToList();
            return PartialView("partials/_getTeachers", svm);
        }

    }
}
