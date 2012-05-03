using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using Quartz; 


namespace TARS.ScheduledJobs
{
    public class TarsScheduledJobs : IJob
    {
        public void Execute(IJobExecutionContext context)
	    {
            switch (context.JobDetail.Description)
            {
                case "lockTimesheets":
                    Debug.WriteLine("lockTimesheets");
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