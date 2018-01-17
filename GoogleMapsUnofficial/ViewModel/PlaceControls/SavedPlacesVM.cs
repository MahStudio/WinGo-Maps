using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace GoogleMapsUnofficial.ViewModel.PlaceControls
{
    class SavedPlacesVM
    {
        /// <summary>
        /// Get saved places in application
        /// </summary>
        /// <returns>Saved Places</returns>
        public static List<SavedPlaceClass> GetSavedPlaces()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<SavedPlaceClass>>(ApplicationData.Current.RoamingSettings.Values["SavedPlaces"].ToString());
            }
            catch 
            {
                return new List<SavedPlaceClass>();
            }
        }

        /// <summary>
        /// Save a place to saved places
        /// </summary>
        /// <param name="Place">Information of the place to save</param>
        /// <returns>return true for success or false</returns>
        /// <exception cref="ArgumentOutOfRangeException">PlaceName is already exists</exception>
        /// <exception cref="Exception">See Exception message for details.</exception>
        public static bool AddNewPlace(SavedPlaceClass Place)
        {
            try
            {
                var r = GetSavedPlaces();
                if (r.Where(x => x.PlaceName.ToLower() == Place.PlaceName.ToLower()).Any())
                    throw new ArgumentOutOfRangeException("PlaceName is already exists");
                r.Add(Place);
                ApplicationData.Current.RoamingSettings.Values["SavedPlaces"] = JsonConvert.SerializeObject(r);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Delete a saved place
        /// </summary>
        /// <param name="PlaceName">name of the saved place</param>
        /// <returns>return true for success or false</returns>
        /// <exception cref="KeyNotFoundException">PlaceName couldn't be found in saved places</exception>
        /// <exception cref="Exception">See Exception message for details.</exception>
        public static bool DeletePlace(string PlaceName)
        {
            try
            {
                var r = GetSavedPlaces();
                var p = r.Where(x => x.PlaceName.ToLower() == PlaceName.ToLower());
                if (p.Count() == 0) throw new KeyNotFoundException("PlaceName not found");
                r.Remove(p.FirstOrDefault());
                ApplicationData.Current.RoamingSettings.Values["SavedPlaces"] = JsonConvert.SerializeObject(r);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class SavedPlaceClass
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string PlaceName { get; set; }
        }
    }
}
