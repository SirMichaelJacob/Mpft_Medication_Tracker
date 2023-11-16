using Med_Tracker.Models;
using System.Linq;


namespace Med_Tracker
{
    public static class ManageAccess
    {
        private static MyDbContext _db = new MyDbContext();

        /// <summary>
        /// Returns User Type either 'Patient' or 'Healthcare Provider'
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string UserType(int? userid)
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


    }
}