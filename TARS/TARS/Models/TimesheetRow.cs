using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TARS.Models
{
    public class TimesheetRow
    {
        public string workeffort { get; set; }
        public string worktype { get; set; }
        public double sunHours { get; set; }
        public double monHours { get; set; }
        public double tueHours { get; set; }
        public double wedHours { get; set; }
        public double thuHours { get; set; }
        public double friHours { get; set; }
        public double satHours { get; set; }
    }

    public class wEffort_wType
    {
        public string workeffort { get; set; }
        public string worktype { get; set; }
    }
}