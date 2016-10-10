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
        public bool IsValid(string _badge)
        {
            var db = new RenExtractEntities().SchoolTeachersWithADLogins;
            var QueryDB = db.FirstOrDefault(x => x.BADGE_NUM == _badge);
            if (QueryDB != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}