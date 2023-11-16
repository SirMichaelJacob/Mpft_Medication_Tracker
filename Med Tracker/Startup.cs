using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Med_Tracker.Startup))]

namespace Med_Tracker
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //GlobalConfiguration.Configuration
            // .UseSqlServerStorage(@"server=localhost;port=3306;userid=root;password=;database=medtrack_api_db;persistsecurityinfo=True");



            GlobalConfiguration.Configuration.UseSqlServerStorage("Data Source=.;Initial Catalog=medtrac_db;Integrated Security=SSPI;");

            //Fire and Forget
            //BackgroundJob.Enqueue(() => FireAndForget());

            //Scheduled jobs
            // BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job method..."), TimeSpan.FromMilliseconds(100000));

            //Recurring jobs
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring job method..."), Cron.Minutely());





            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
