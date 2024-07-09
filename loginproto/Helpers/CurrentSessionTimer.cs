using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace loginproto.Helpers
{
    public static class CurrentSessionTimer
    {
        public static int IsSummerCamp(DateTime currentDate)
        {
            DateTime summerCampStart = new DateTime(currentDate.Year, 6, 20);
            DateTime summerCampEnd = new DateTime(currentDate.Year, 8, 25);

            if (currentDate >= summerCampStart && currentDate <= summerCampEnd)
            {
                // Summer camp duration: 3 hours and 5 minutes
                return 3 * 60 * 60 + 5 * 60; // 3 hours and 5 minutes in seconds
            }
            else
            {
                // Regular duration: 1 hour and 45 minutes
                return 1 * 60 * 60 + 45 * 60; // 1 hour and 45 minutes in seconds
            }

        }
    }
}
