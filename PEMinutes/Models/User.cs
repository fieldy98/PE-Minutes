using System.ComponentModel.DataAnnotations;
using System.Linq;
using PEMinutes.EF;

namespace PEMinutes.Models
{
    
    public class User
    {
        private readonly PEMinutesEntities _db = new PEMinutesEntities();

        [Required]
        [Display(Name = "Badge Number")]
        public string StaffBadgeNumber { get; set; }
        //public bool IsTeacher(string badge)
        //{
        //    var queryDb = _db.SchoolTeachersWithADLogins.FirstOrDefault(x => x.BADGE_NUM == badge);
        //    return queryDb != null;
        //}
        public bool IsPrincipal(string badge)
        {
            
            var queryDb = _db.SchoolToPrincipals.FirstOrDefault(x => x.BADGE_NUM == badge);
            return queryDb != null;
        }
        public bool IsAdmin(string badge)
        {
            var queryDb = _db.MinutesAdmins.FirstOrDefault(x => x.BADGE_NUM == badge);
            return queryDb != null;
        }
    }
}