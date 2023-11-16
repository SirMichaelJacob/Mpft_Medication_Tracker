using Med_Tracker.Models;
using System.Linq;

namespace System.Web.Mvc
{
    public class AuthorizeUserAccess : AuthorizeAttribute
    {
        MyDbContext _db = new MyDbContext();


        public string myRole { get; set; } //The specified Access Role

        //private Context db = new Context();
        protected override bool AuthorizeCore(HttpContextBase curContext)
        {
            bool UserIsProvider = false;
            var isAuthorized = base.AuthorizeCore(curContext);
            if (!isAuthorized)
            {
                UserIsProvider = false;
            }

            int currentUserName = int.Parse(HttpContext.Current.User.Identity.Name); //Currrent LoggedIn User ID
            string loggedInUserEmail = (string)HttpContext.Current.Session["Username"];

            Provider provider = _db.Providers.Where(x => x.ProviderId == currentUserName).SingleOrDefault();


            if (provider == null)
            {
                UserIsProvider = false;
            }
            else
            {
                if (loggedInUserEmail == null)
                {
                    HttpContext.Current.Session["Username"] = provider.Email;
                    HttpContext.Current.Session.Add("Username", provider.Email);
                    UserIsProvider = true;
                }

            }

            return UserIsProvider;
        }



    }
}