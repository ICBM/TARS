using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using TARS.Controllers;
using Quartz;

//NOTE: Scheduled jobs are specified in ConfigureQuartzJobs(), located in Global.asax

namespace TARS.ScheduledJobs
{
    public class TarsScheduledJobs : AdminController, IJob
    {
        public void Execute(IJobExecutionContext context)
	    {
            switch (context.JobDetail.Description)
            {
                case "lockTimesheets":
                    lockTimesheets();
                    Debug.WriteLine("lockingTimesheets");
                    break;
                case "remindSubmitTimesheet":
                    reminderToSubmitTimesheet();
                    Debug.WriteLine("remindSubmitTimesheet");
                    break;
                case "remindPcaOrWeExpires":
                    notificationOfPcaOrWeExpiration();
                    Debug.WriteLine("remindPcaOrWeExpires");
                    break; 
            }
	    }
    }
}