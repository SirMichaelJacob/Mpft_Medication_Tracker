using Med_Tracker.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Med_Tracker.Controllers
{
    [Authorize]
    public class MedicationController : Controller
    {
        MyDbContext _db = new MyDbContext();

        //Get the LoggedIn Patient


        public MedicationController()
        {
            // Get a list of medication types
            var medicationTypes = new List<string>()
            {
                "Analgesics",
                "Antibiotics",
                "Antidepressants",
                "Antihistamines",
                "Decongestants",
                "Cough suppressants",
                "Expectorants",
                "Laxatives",
                "Nasal sprays",
                "Ointments",
                "Patches",
                "Powders"
            };


            //Medication Frequency
            var frequency = new List<string>()
            {
                "PRN (As Needed)",
                "Once Daily (24 hourly)",
                "Twice Daily (12 hourly)",
                "Thrice Daily (8 hourly)",
                "Four times Daily (6 hourly)",
                "Once a week"
            };

            SelectList medList = new SelectList(medicationTypes);
            SelectList freqList = new SelectList(frequency);

            // Pass the list of medication types to the view
            ViewBag.MedicationTypes = medList;
            ViewBag.Frequency = freqList;
        }
        // GET: Medication
        public ActionResult Index()
        {

            var userId = int.Parse(HttpContext.User.Identity.Name);
            if (ManageAccess.UserType(userId) == "Patient")
            {
                List<Medication> list = _db.Medications.Where(x => x.PatientId == userId).ToList();


                List<Medication> patientMedicationsList = new List<Medication>();
                List<Medication> allMedications = _db.Medications.ToList();

                foreach (Medication med in allMedications)
                {
                    if (med.PatientId == userId)
                    {
                        patientMedicationsList.Add(med);
                    }
                }
                //
                ViewBag.Patient = _db.Patients.Where(x => x.PatientId == userId);

                return View(patientMedicationsList);
            }

            //List<Patient> allPatients = _db.Patients.ToList();

            return RedirectToAction("Index", "Patient");
        }

        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        /* Add New Medication*/
        [HttpPost]

        public ActionResult New(Medication medication)
        {
            try
            {
                if (ManageAccess.UserType(int.Parse(HttpContext.User.Identity.Name)) == "Patient")
                {
                    medication.PatientId = int.Parse(HttpContext.User.Identity.Name); //Get Logged In Patient ID
                }
                else
                {
                    medication.PatientId = int.Parse(HttpContext.User.Identity.Name); //Get Logged In Patient ID
                }

                _db.Medications.Add(medication);
                _db.SaveChanges();

                //Schedule the Notification using hangfire
                BackgroundTaskManager.ScheduleEmailNotification(medication);

                return RedirectToAction("Index");


            }
            catch (System.Exception)
            {

                return View();
            }
        }


        [HttpGet]
        public ActionResult NewMed(int patientId)
        {
            return View("New");
        }



        [Authorize]

        [HttpPost]

        public ActionResult NewMed(Medication medication, int patientId)
        {
            try
            {


                medication.PatientId = patientId;


                _db.Medications.Add(medication);
                _db.SaveChanges();

                //Schedule the Notification using hangfire
                BackgroundTaskManager.ScheduleEmailNotification(medication);

                return RedirectToAction("Index");


            }
            catch (System.Exception)
            {

                return View("New");
            }
        }

        /*Edit*/

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Medication model = _db.Medications.SingleOrDefault(x => x.MedicationId == id);
            if (model != null)
            {
                return View(model);
            }

            return View("ErrorPage");
        }

        [HttpPost]
        public ActionResult Edit(Medication medication)
        {
            try
            {
                _db.Entry(medication).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index");

            }
            catch (System.Exception)
            {
                return View("ErrorPage");
            }
        }

        /*Delete*/
        //Note that I do not need to specify [HttpDelete] attribute because Ive done a Prefix with Delete
        public ActionResult Delete(int id)
        {
            try
            {
                Medication medication = _db.Medications.Find(id);
                if (medication != null)
                {
                    _db.Medications.Remove(medication);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception)
            {

                return View("ErrorPage");
            }
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



        /////////////////////
        public ActionResult Users(int id)
        {
            if (_db.Patients.Where(x => x.PatientId == id).SingleOrDefault() != null)
            {
                List<Medication> user_med_list = _db.Medications.Where(x => x.PatientId == id).ToList();

                ViewBag.UserID = id;
                return View(user_med_list);
            }
            return View("ErrorPage");
        }
        ///

    }
}