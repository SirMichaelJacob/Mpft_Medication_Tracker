using Med_Tracker.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace Med_Tracker.Controllers
{

    public class ProviderController : Controller
    {
        static string _salt = "MIDLANDS";
        MyDbContext _db = new MyDbContext();
        // GET: Provider
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        /* Add New Provider*/
        [HttpPost]

        public ActionResult Register(Provider provider)
        {
            if (!IsEmailAlreadyUsed(provider.Email))
            {

                //
                provider.Confirmed = "NO";
                //Generate Unique registration token
                provider.RegToken = GenerateRegistrationToken();
                //

                //Hash Password
                string hashedPWord = Crypto.HashPassword(provider.PasswordHash + _salt);
                provider.PasswordHash = hashedPWord;
                provider.Re_PasswordHash = hashedPWord;
                //
                _db.Providers.Add(provider);
                _db.SaveChanges();

                //Send Registration and Confirmation mail
                SendConfirmationEmail(provider.Email, provider.RegToken);

                //return RedirectToAction("Index");
                return View("Success", provider);

            }
            else
            {
                ModelState.AddModelError("", "Email is already registered");
                return View();
            }


        }

        //Edit Provider
        [AuthorizeUserAccess]
        public ActionResult Edit(int id)
        {
            Provider provider = _db.Providers.SingleOrDefault(x => x.ProviderId == id);
            if (provider == null)
            {
                return View("ErrorPage");
            }

            return View("Register", provider);
        }


        /*Edit*/
        [AuthorizeUserAccess]
        [HttpPut]
        public ActionResult Edit(Provider provider)
        {

            if (provider != null)
            {
                _db.Entry(provider).State = EntityState.Modified;
                _db.SaveChanges();


                return RedirectToAction("Index");
            }
            else
            {
                return View("Register");
            }

        }



        /*Delete*/
        //Note that I do not need to specify [HttpDelete] attribute because Ive done a Prefix with Delete
        [AuthorizeUserAccess]
        public ActionResult Delete(int id)
        {
            try
            {
                Provider Provider = _db.Providers.Find(id);
                if (Provider != null)
                {
                    _db.Providers.Remove(Provider);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception)
            {

                return RedirectToAction("Index"); ;
            }
        }





        private string GenerateRandomAlphanumeric(int length)
        {
            const string alphanumericChars = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(alphanumericChars[random.Next(alphanumericChars.Length)]);
            }

            return result.ToString();
        }


        /*Generate Reg token*/

        // Method to generate a registration token
        private string GenerateRegistrationToken()
        {
            // Generate a unique identifier using GUID
            string uniqueId = Guid.NewGuid().ToString();

            // Generate a random password using Membership.GeneratePassword
            string password = Membership.GeneratePassword(10, 3);

            // Concatenate the unique identifier and password to create the token
            string token = uniqueId + password;

            return token;
        }


        private bool IsEmailAlreadyUsed(string email)
        {
            Provider provider = _db.Providers.Where(x => x.Email.ToLower() == email.ToLower()).SingleOrDefault();
            Patient patient = _db.Patients.Where(x => x.Email.ToLower() == email.ToLower()).SingleOrDefault();
            if (provider != null || patient != null)
            {

                return true;
            }
            else
            {
                return false;
            }
        }


        // Method to send the confirmation email
        private void SendConfirmationEmail(string userEmail, string myToken)
        {
            // Configure the SMTP settings for your email provider
            //SmtpClient smtpClient = new SmtpClient("smtp.yourprovider.com", 587);
            SmtpClient smtpClient = new SmtpClient("mail.ccpgroup.com.ng", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("info@ccpgroup.com.ng", "Mykel101#");

            // Create the email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("info@ccpgroup.com.ng"); // Replace with your email address
            mailMessage.To.Add(userEmail);
            mailMessage.Subject = "Midlands Med. Tracker Healthcare provider Account Confirmation";
            mailMessage.Body = "Dear Healthcare provider, please click the link to confirm your account: " +
                               "<a href='" + Url.Action("ConfirmAccount", "Provider", new { email = userEmail, regToken = myToken }, Request.Url.Scheme) + "'>Confirm Account</a>";

            // Set the email body as HTML
            mailMessage.IsBodyHtml = true;
            Url.Action("", "", "", "");

            // Send the email
            smtpClient.Send(mailMessage);
        }

        // Method to validate the registration token
        private bool ValidateToken(string email, string token)
        {
            bool isValid = false;
            // Implement your token validation logic here
            var Provider = new Provider();
            var all_Providers = _db.Providers.ToList();
            foreach (Provider p in all_Providers)
            {
                if (p.Email == email)
                {
                    Provider = p;
                }
            }
            //var user = _db.UserAccounts.Where(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            // Compare the provided token with the one stored in the database
            if (Provider.RegToken.Equals(token))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            // Return true if the token is valid; otherwise, return false
            return isValid;
        }

        // Action method to confirm the user account
        public ActionResult ConfirmAccount(string email, string regToken)
        {

            var model = FindProvider(email);

            // Validate the token and confirm the user account
            if (ValidateToken(model.Email, model.RegToken))
            {
                model.Confirmed = "YES";
                _db.Entry(model).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("ConfirmationSuccess", new { email = model.Email });
            }
            else
            {
                // Token validation failed
                // Redirect to an error page or display an error message

                return RedirectToAction("ConfirmationError");
            }
        }

        public ActionResult ConfirmationSuccess(string email)
        {
            ViewBag.Result = "Account Verified! Your registered email is: " + email.ToLower() + ". Please keep it Safe!";
            return View();
        }

        public ActionResult Success(Provider provider)
        {
            return View(provider);
        }

        public ActionResult ConfirmationError()
        {
            ViewBag.Result = "Verification failed";

            return View();
        }

        public Provider FindProvider(String email)
        {
            var model = new Provider();
            var all_Providers = _db.Providers.ToList();
            foreach (Provider p in all_Providers)
            {
                if (p.Email.ToLower() == email.ToLower())
                {
                    model = p;
                    break;
                }
            }
            return model;
        }


    }
}