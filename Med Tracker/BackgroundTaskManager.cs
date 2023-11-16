using Hangfire;
using Med_Tracker.Models;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Med_Tracker
{

    public class BackgroundTaskManager
    {
        public static MyDbContext _db = new MyDbContext();



        public static void ScheduleEmailNotification(Medication medication)
        {
            RecurringJob.AddOrUpdate(
                $"medication-{medication.MedicationId}",
                () => SendEmailNotification(medication),
                setFrequency(medication)

            );
        }

        public static string setFrequency(Medication medication)
        {
            if (medication.Frequency == "Once Daily (24 hourly)")
            {
                return Cron.Daily();
            }
            if (medication.Frequency == "Twice Daily (12 hourly)")
            {
                return Cron.HourInterval(12);
            }
            if (medication.Frequency == "Thrice Daily (8 hourly)")
            {
                return Cron.HourInterval(8);
            }
            if (medication.Frequency == "Four times Daily (6 hourly)")
            {
                return Cron.HourInterval(6);
            }
            if (medication.Frequency == "Once a week")
            {
                return Cron.Weekly();
            }
            return Cron.Never(); //For PRN (As needed)
        }

        public static void SendEmailNotification(Medication medication)
        {
            Patient patient = _db.Patients.SingleOrDefault(x => x.PatientId == medication.PatientId);


            // Configure the SMTP settings for your email provider
            //SmtpClient smtpClient = new SmtpClient("smtp.yourprovider.com", 587);
            SmtpClient smtpClient = new SmtpClient("mail.ccpgroup.com.ng", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("info@ccpgroup.com.ng", "Mykel101#");

            // Create the email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("info@ccpgroup.com.ng"); // Replace with your email address
            mailMessage.To.Add(patient.Email);
            mailMessage.Subject = "Med. Tracker Medication Alert!";
            mailMessage.Body = "Dear " + patient.LastName + ", Its time to take your medication. Medication Name:" + medication.MedicationName + ". Medication Type: " + medication.MedicationType
                + ". Open the application for more details";

            // Set the email body as HTML
            mailMessage.IsBodyHtml = true;
            //Url.Action("", "", "", "");

            // Send the email
            smtpClient.Send(mailMessage);



        }
    }
}