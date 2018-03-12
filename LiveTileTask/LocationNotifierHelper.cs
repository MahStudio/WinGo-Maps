using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace LiveTileTask
{
    class LocationNotifierHelper
    {
        public class LocBasedReminderClass
        {
            public BasicGeoposition Location { get; set; }
            public string Reminder { get; set; }
        }

        public static void AddReminder(LocBasedReminderClass Reminder)
        {

        }

        public static List<LocBasedReminderClass> GetAvailableReminders()
        {
            return null;
        }

        public static void RemoveReminder(LocBasedReminderClass Reminder)
        {

        }
    }
}
