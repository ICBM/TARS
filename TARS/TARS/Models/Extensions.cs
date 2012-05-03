using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TARS.Models
{
    public static class DateTimeExtensions
    {
        //method that returns the DateTime of the weekDay provided
        //example: DateTime.Now.StartOfWeek(DayOfWeek.Sunday) will return the DateTime of the Sunday of this week
        public static DateTime StartOfWeek(this DateTime referenceDate, DayOfWeek WeekDay)
        {
            int diff = referenceDate.DayOfWeek - WeekDay;
            if (diff < 0)
            {
                diff += 7;
            }
            return referenceDate.AddDays(-1 * diff).Date;
        }
    }
}