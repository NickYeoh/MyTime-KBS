using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTime.Interfaces
{
    interface ISharedFunction
    {
        int CalculateMonthDiff(DateTime firstDate, DateTime secondDate);
    }

    public class SharedFunction : ISharedFunction
    {
        public int CalculateMonthDiff(DateTime firstDate, DateTime secondDate)
        {
            int monthsApart = 12 * (firstDate.Year - secondDate.Year) + firstDate.Month - secondDate.Month;
            return Math.Abs(monthsApart);
        }
    }

  
}
