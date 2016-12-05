using System.ComponentModel.DataAnnotations;
using System.Linq;
using PEMinutes.EF;

namespace PEMinutes.Models
{
    public class User
    {
        [Required]
        [Display(Name = "Badge Number")]
        public string StaffBadgeNumber { get; set; }
        public bool IsTeacher(string badge)
        {
            var db = new RenExtractEntities().SchoolTeachersWithADLogins;
            var queryDb = db.FirstOrDefault(x => x.BADGE_NUM == badge);
            return queryDb != null;
        }
        public bool IsPrincipal(string badge)
        {
            var db = new RenExtractEntities().SchoolToPrincipals;
            var queryDb = db.FirstOrDefault(x => x.BADGE_NUM == badge);
            return queryDb != null;
        }
        public bool IsAdmin(string badge)
        {
            var db = new RenExtractEntities().MinutesAdmins;
            var queryDb = db.FirstOrDefault(x => x.BADGE_NUM == badge);
            return queryDb != null;
        }
    }
}