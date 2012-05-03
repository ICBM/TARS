using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TARS.Models
{
    public class TimesheetRow
    {
        public string workeffort { get; set; }
        public int weID { get; set; }
        public string timecode { get; set; }
        public Hours sunHours { get; set; }
        public Hours monHours { get; set; }
        public Hours tueHours { get; set; }
        public Hours wedHours { get; set; }
        public Hours thuHours { get; set; }
        public Hours friHours { get; set; }
        public Hours satHours { get; set; }
    }
}