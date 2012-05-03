using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using TARS.Controllers;
using Quartz; 


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
                    Debug.WriteLine("remindSubmitTimesheet");
                    break;
                case "remindPcaOrWeExpires":
                    Debug.WriteLine("remindPcaOrWeExpires");
                    break; 
            }
	    }
    }
}