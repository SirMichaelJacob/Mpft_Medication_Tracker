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
    public class AdminController : Controller
    {
        static string _salt = "MIDLANDS";
        MyDbContext _db = new MyDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Medication");
            }
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index(Provider model)
        {
            var allProviders = _db.Providers.ToList();
            Provider user = null;
            foreach (var provider in allProviders)
            {
                if (provider.Email.ToUpper() == model.Email.ToUpper() && Crypto.VerifyHashedPassword(provider.PasswordHash, model.PasswordHash + _salt))
                {

                    user = provider;
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

        public ActionResult Welcome(Provider model)
        {
            return View(model);
        }

        public List<Provider> GetAllProviders()
        {
            List<Provider> providersList = _db.Providers.ToList();

            return providersList;
        }

        //User for Healthcare providers Authentication
        public void AuthenticateUser(Provider account)
        {
            FormsAuthentication.SetAuthCookie(account.ProviderId.ToString(), true);

            var serializedUser = Newtonsoft.Json.JsonConvert.SerializeObject(account);
            var ticket = new FormsAuthenticationTicket(1, account.ProviderId.ToString(), DateTime.Now, DateTime.Now.AddHours(3), true, serializedUser);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var isSsl = Request.IsSecureConnection; // if we are running in SSL mode then make the cookie secure only

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true, // always set this to true!
                Secure = isSsl,
            };

            Response.Cookies.Set(cookie);


            var currentUserID = HttpContext.User.Identity.Name;
            Session["UserID"] = account.ProviderId;
            HttpContext.Session["Username"] = account.Email;
            HttpContext.Session.Add("Username", account.Email);




        }
    }
}