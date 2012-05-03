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
            if (true)
            {
                Debug.WriteLine("Hello at " + DateTime.Now.ToString() + " --- " + context);
            }
	    }
    }
}