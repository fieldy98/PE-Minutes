using System.Web.Mvc;
using System.Web.Security;

//
// OVERVIEW:
//      This controller handles the login/logout of users throughout the application.
//      It checks with the 'User' model for verification of the staff members entered badge.
//

namespace PEMinutes.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Return Teacher/Sub select screen
        public ActionResult Index()
        {
            return View();
        }
        // GET: Return Login Page
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        // POST: Authenticatate user
        // If the user is valid, create a cookie and return the teacher landing page.
        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            // Checks with the 'User' model and runs the users input through the IsValid method.
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.StaffBadgeNumber))
                {
                    // If a match is found, create a cookie and return the teacher homepage
                    FormsAuthentication.SetAuthCookie(user.StaffBadgeNumber, true);
                    return RedirectToAction("Index", "Teacher");
                }
                else
                {
                    // If no match found, report error.
                    ModelState.AddModelError("", "No staff member found with that Badge Number.");
                }
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            // Expire Cookie and redirect to Teacher/Sub page
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Authentication");
        }

    }
}