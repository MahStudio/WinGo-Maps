using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

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
            try
            {
            }
            catch 
            {
                
            }
        }

        public static List<LocBasedReminderClass> GetAvailableNearbyReminders(BasicGeoposition UserLocation)
        {
            try
            {
                var res = ApplicationData.Current.LocalSettings.Values["AvailableReminders"].ToString();
                //var AvailableReminders = JsonConvert.DeserializeObject<List<LocBasedReminderClass>>(res);
                //var NearReminders = AvailableReminders.Where(x => x.Location.DistanceTo(UserLocation) <= 1);
                return new List<LocBasedReminderClass>();
            }
            catch
            {
                return new List<LocBasedReminderClass>();
            }
        }

        public static void RemoveReminder(LocBasedReminderClass Reminder)
        {
            try
            {
                var res = ApplicationData.Current.LocalSettings.Values["AvailableReminders"].ToString();
                //var AvailableReminders = JsonConvert.DeserializeObject<List<LocBasedReminderClass>>(res);
                //AvailableReminders.Remove(Reminder);
            }
            catch
            {
            }
        }
    }
}
