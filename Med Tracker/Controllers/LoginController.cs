using Med_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace Med_Tracker.Controllers
{
    public class LoginController : Controller
    {
        static string _salt = "MIDLANDS";
        MyDbContext _db = new MyDbContext();

        [AllowAnonymous]
        // GET: Login
        public ActionResult Index()
        {
            List<string> sessionVariableNames = new List<string>();
            if (User.Identity.IsAuthenticated)
            {

                //Check if LoggedIn User is a Healthcare Provider
                int currentUserName = int.Parse(HttpContext.User.Identity.Name); //Currrent LoggedIn User ID
                string loggedInUserEmail = (string)HttpContext.Session["Username"];

                Provider provider = _db.Providers.Where(x => x.ProviderId == currentUserName && x.Email.ToLower() == loggedInUserEmail).SingleOrDefault();
                if (provider != null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Account");
            }
            return View();

        }


        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index(Patient model)
        {
            var allPatients = _db.Patients.ToList();
            Patient user = null;
            foreach (var patient in allPatients)
            {
                if (patient.NHSNumber.ToUpper() == model.NHSNumber.ToUpper() && Crypto.VerifyHashedPassword(patient.PasswordHash, model.PasswordHash + _salt))
                {

                    user = patient;
                    break;
                }
            }
            if (user != null)
            {
                HttpContext.Session.Clear();
                Session.Abandon(); //End All previous sessions

                AuthenticateUser(user);
                return RedirectToAction("Index", "Account");
                //return Content(HttpContext.User.Identity.Name);

            }
            else
            {
                ModelState.AddModelError("", "Invalid Login!");
            }
            return View();
        }

        public ActionResult Welcome(Patient model)
        {
            return View(model);
        }

        public List<Patient> GetAllPatients()
        {
            List<Patient> patientsList = _db.Patients.ToList();

            return patientsList;
        }

        //User Authentication
        public void AuthenticateUser(Patient account)
        {
            FormsAuthentication.SetAuthCookie(account.PatientId.ToString(), true);

            var serializedUser = Newtonsoft.Json.JsonConvert.SerializeObject(account);
            var ticket = new FormsAuthenticationTicket(1, account.PatientId.ToString(), DateTime.Now, DateTime.Now.AddHours(3), true, serializedUser);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var isSsl = Request.IsSecureConnection; // if we are running in SSL mode then make the cookie secure only

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true, // always set this to true!
                Secure = isSsl,
            };

            Response.Cookies.Set(cookie);


            var currentUserID = HttpContext.User.Identity.Name;
            //Session["UserID"] = account.PatientId.ToString();
            Session["UserID"] = account.NHSNumber;
            HttpContext.Session["Username"] = account.Email;
            HttpContext.Session.Add("Username", account.Email);


        }
    }
}