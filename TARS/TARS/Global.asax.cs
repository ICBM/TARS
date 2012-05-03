using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using TARS.Controllers;
using TARS.Models;

using Quartz;   //for scheduling jobs
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace TARS 
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        //
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = -1 } // Parameter defaults
			
/*			
				//the conditional statement makes the URL routing work with both IIS6 and IIS7
				"Default",
				usingIntegratedPipeline ?
					"{controller}/{action}/{id}" : "{controller}.mvc/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } //Parameter defaults
*/
            );
        }


        //
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // start the job scheduler
            ConfigureQuartzJobs(); 
        }


        //
        // Uses Quartz.NET package to schedule jobs
        public static void ConfigureQuartzJobs()
        {
	        // construct a scheduler factory
	        ISchedulerFactory schedFact = new StdSchedulerFactory();
	 
	        // get a scheduler
	        IScheduler sched = schedFact.GetScheduler();
	        sched.Start();

            /* Trigger to lock timesheets every Tuesday and Wednesday morning at 12:00am.
             * Timesheets won't be locked on Tuesday if there was a holiday, and won't be
             * locked on Wednesday if there wasn't a holiday
             */
            JobDetailImpl job1 = new JobDetailImpl("lockTimesheets", typeof(TARS.ScheduledJobs.TarsScheduledJobs));
            CronTriggerImpl lockTimesheets = new CronTriggerImpl("lock",
                                                                  null,
                                                                  "lockTimesheets",
                                                                  null,
                                                                  "0 0 0 ? * 'TUE,WED'"
                                                                );
            sched.ScheduleJob(job1, lockTimesheets);

            // trigger to send reminder email to employees to submit timesheets by end of day Saturday
            JobDetailImpl job2 = new JobDetailImpl("submitTsRemind", typeof(TARS.ScheduledJobs.TarsScheduledJobs));
            CronTriggerImpl submitTimesheetReminder = new CronTriggerImpl("submitReminder",
                                                                           null,
                                                                           "submitTsRemind",
                                                                           null,
                                                                           "0 0 0 ? * SAT"
                                                                         );
            sched.ScheduleJob(job2, submitTimesheetReminder);

            // trigger to send reminder email to managers and admin when a PCA or Work Effort expire that week
            JobDetailImpl job3 = new JobDetailImpl("pcaAndWeExpRemind", typeof(TARS.ScheduledJobs.TarsScheduledJobs)); 
            CronTriggerImpl pcaAndWeExpirationReminder = new CronTriggerImpl("pcaAndWeReminder",
                                                                              null,
                                                                              "pcaAndWeExpRemind",
                                                                              null,
                                                                              "0 0 0 ? * SUN"
                                                                            );
            sched.ScheduleJob(job3, pcaAndWeExpirationReminder);
	    }
    }
}