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

        static string _salt = "MIDLANDS";
        MyDbContext _db = new MyDbContext();


        public AccountController()
        {

        }


        // GET: Account
        [Authorize]
        public ActionResult Index()
        {


            return RedirectToAction("Index", "Medication");
        }

        public string UserType(int? userid)
        {
            var result = "Null";
            if (userid != null)
            {

                var user = _db.Patients.Where(x => x.PatientId.ToString() == userid.ToString()).FirstOrDefault();
                var admin = _db.Providers.Where(x => x.ProviderId.ToString() == userid.ToString()).FirstOrDefault();

                if (user != null)
                {
                    result = "Patient";
                }
                if (admin != null)
                {
                    result = "Provider";
                }

                return result;

            }
            else
            {
                return result;
            }


        }


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