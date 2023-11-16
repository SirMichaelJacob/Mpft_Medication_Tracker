using Med_Tracker.Models;
using System;
using System.Collections.Generic;
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
    //[AuthorizeUserAccess]

    public class PatientController : Controller
    {
        static string _salt = "MIDLANDS";
        MyDbContext _db = new MyDbContext();


        //Index
        [Authorize]
        public ActionResult Index()
        {
            var userId = int.Parse(HttpContext.User.Identity.Name);
            if (ManageAccess.UserType(userId) != "Patient")
            {

                // Retrieve patient and medication data from the database
                List<Patient> patients_list = _db.Patients.ToList();
                List<Medication> medications_list = _db.Medications.ToList();

                //// Create an instance of the view model
                var viewModel = new AdminViewModel()
                {
                    Patients = patients_list,
                    Medications = medications_list
                };

                ViewBag.Patients = patients_list;
                ViewBag.Medications = medications_list;

                // Pass the view model to the view
                return View("Home", viewModel);

                //return View(patients_list);
            }
            return RedirectToAction("Index", "Medication");
        }

        /// <summary>
        /// Action Method (Get) to load patient registration form
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Action Method (Post) to Create new Patient account
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(Patient patient)
        {
            if (!IsEmailAlreadyUsed(patient.Email))
            {
                // Generate unique alphanumeric NHS number
                string nhsNumber = GenerateUniqueNHSNumber();
                patient.NHSNumber = nhsNumber;
                //
                patient.Confirmed = "NO";
                //Generate Unique registration token
                patient.RegToken = GenerateRegistrationToken();
                //

                //Hash Password
                string hashedPWord = Crypto.HashPassword(patient.PasswordHash + _salt);
                patient.PasswordHash = hashedPWord;
                patient.Re_PasswordHash = hashedPWord;
                //
                _db.Patients.Add(patient);
                _db.SaveChanges();

                //Send Registration and Confirmation mail
                /*Un-coment the next line to send registration email*/
                //SendConfirmationEmail(patient.Email, patient.RegToken);

                //return RedirectToAction("Index");
                return View("RegSuccess", patient);

            }
            else
            {
                ModelState.AddModelError("", "Email is already registered");
                return View();
            }


        }

        /// <summary>
        /// Action Method (Get) to retrieve Patient data and load to view for editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int id)
        {
            Patient patient = _db.Patients.SingleOrDefault(x => x.PatientId == id);
            var userId = int.Parse(HttpContext.User.Identity.Name);
            if (id == userId)
            {
                if (patient == null)
                {
                    return View("ErrorPage");
                }
                return View("Register", patient);
            }
            return View("AccessDenied");


        }


        /// <summary>
        /// Action Method to Update Patient account data
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public ActionResult Edit(Patient patient)
        {

            if (patient != null)
            {
                _db.Entry(patient).State = EntityState.Modified;
                _db.SaveChanges();


                return RedirectToAction("Index");
            }
            else
            {
                return View("Register");
            }

        }


        /// <summary>
        /// Delete Action method to remove Patient account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /*Delete*/
        //Note that I do not need to specify [HttpDelete] attribute because Ive done a Prefix with Delete
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                Patient Patient = _db.Patients.Find(id);
                if (Patient != null)
                {
                    _db.Patients.Remove(Patient);
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




        /// <summary>
        /// * Method to generate Alpha numeric NHS number*
        /// </summary>
        /// <returns></returns>
        private string GenerateUniqueNHSNumber()
        {
            string nhsNumber;
            do
            {
                nhsNumber = GenerateRandomAlphanumeric(10);
            } while (IsNHSNumberAlreadyUsed(nhsNumber));

            return nhsNumber;
        }


        /// <summary>
        /// Method to Generate Alphanumeric Random Number
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Method to check if NHS Number is Unique 
        /// </summary>
        /// <param name="nhsNumber"></param>
        /// <returns></returns>
        private bool IsNHSNumberAlreadyUsed(string nhsNumber)
        {
            return _db.Patients.Any(u => u.NHSNumber == nhsNumber);
        }


        /// <summary>
        /// Method to generate a registration token for User registration.
        /// This token will be used to verify user account
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Method to check if Email address exists in the database
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool IsEmailAlreadyUsed(string email)
        {
            return _db.Patients.Any(u => u.Email.ToLower() == email.ToLower());
        }


        /// <summary>
        /// Method to send the confirmation email to user mail address.
        /// This method requires the user email and registration token
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="myToken"></param>
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
            mailMessage.Subject = "Midlands Med. Tracker Account Confirmation";
            mailMessage.Body = "Dear User, please click the link to confirm your account: " +
                               "<a href='" + Url.Action("ConfirmAccount", "Patient", new { email = userEmail, regToken = myToken }, Request.Url.Scheme) + "'>Confirm Account</a>";

            // Set the email body as HTML
            mailMessage.IsBodyHtml = true;
            Url.Action("", "", "", "");

            // Send the email
            smtpClient.Send(mailMessage);
        }

        /// <summary>
        /// Method to validate the registration token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool ValidateToken(string email, string token)
        {
            bool isValid = false;
            // Implement your token validation logic here
            var patient = new Patient();
            var all_patients = _db.Patients.ToList();
            foreach (Patient p in all_patients)
            {
                if (p.Email == email)
                {
                    patient = p;
                }
            }
            //var user = _db.UserAccounts.Where(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            // Compare the provided token with the one stored in the database
            if (patient.RegToken.Equals(token))
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

        /// <summary>
        /// Action method to confirm the user account.
        /// Sets the CONFIRMED field in Database to 'YES'
        /// </summary>
        /// <param name="email"></param>
        /// <param name="regToken"></param>
        /// <returns></returns>
        public ActionResult ConfirmAccount(string email, string regToken)
        {

            var model = FindPatient(email);

            // Validate the token and confirm the user account
            if (ValidateToken(model.Email, model.RegToken))
            {
                model.Confirmed = "YES";
                _db.Entry(model).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("ConfirmationSuccess", new { nhsNumber = model.NHSNumber });
            }
            else
            {
                // Token validation failed
                // Redirect to an error page or display an error message

                return RedirectToAction("ConfirmationError");
            }
        }

        public ActionResult ConfirmationSuccess(string nhsNumber)
        {
            ViewBag.Result = "Account Verified! Your NHS Number is: " + nhsNumber + ". Please keep it Safe!";
            return View();
        }

        public ActionResult RegSuccess(Patient patient)
        {
            return View(patient);
        }

        public ActionResult ConfirmationError()
        {
            ViewBag.Result = "Verification failed";

            return View();
        }


        /// <summary>
        /// Find patient in database and returns Patient Object 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Patient FindPatient(String email)
        {
            var model = new Patient();
            var all_patients = _db.Patients.ToList();
            foreach (Patient p in all_patients)
            {
                if (p.Email == email)
                {
                    model = p;
                    break;
                }
            }
            return model;
        }

    }
}