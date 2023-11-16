using Med_Tracker.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Med_Tracker.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        static string _salt = "MIDLANDS"; //Password Salt to improve password strength
        readonly MyDbContext _db = new MyDbContext(); //ReadOnly DbContext Object



        // GET: Account
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Medication"); //Redirect LoggedIn Patients to Medication Area
        }

        /// <summary>
        /// Return User First name and Lastname
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string getUser(int? userid)
        {
            var result = "Null";
            if (userid != null)
            {

                var user = _db.Patients.Where(x => x.PatientId.ToString() == userid.ToString()).FirstOrDefault();
                var admin = _db.Providers.Where(x => x.ProviderId.ToString() == userid.ToString()).FirstOrDefault();

                if (user != null)
                {
                    result = String.Format("{0} {1}", user.FirstName, user.LastName);
                }
                if (admin != null)
                {
                    result = admin.Name;
                }

                return result;

            }
            else
            {
                return result;
            }


        }


        /// <summary>
        /// LogOut Method.
        /// Sign the User out and clears all sessions
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}