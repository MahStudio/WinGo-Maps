using GMapsUWP.GeoCoding;
using GMapsUWP.Place;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Geolocation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using WinGoMapsX.ViewModel.PlacesControls;
using WinGoMapsX.ViewModel.SettingsView;

namespace WinGoMapsX.ViewModel.OnMapControls
{
    class SearchVMClass : INotifyPropertyChanged
    {
        public MapControl Map { get; set; }
        public string SearchQuerry
        {
            set
            {
                SuggestForSearch(value);
            }
        }

        private void SuggestedApps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("SearchResults");
        }

        private ObservableCollection<PlaceAutoComplete.Prediction> _searchres;

        public ObservableCollection<PlaceAutoComplete.Prediction> SearchResults
        {
            get
            {
                return _searchres;
            }
            set
            {
                _searchres = value;
                OnPropertyChanged("SearchResults");
            }
        }

        public async void NearbySearchSuggestions()
        {
            var res = await PlaceSearchHelper.NearbySearch(new BasicGeoposition()
            {
                Longitude = Map.Center.Position.Longitude,
                Latitude = Map.Center.Position.Latitude
            }, 1500);
            if (res == null || res.Results == null || res.Results.Length ==0) return;
            SearchResults.Clear();
            List<MapIcon> MapIcons = new List<MapIcon>();
            foreach (var item in Map.MapElements)
            {
                if (item.GetType() == typeof(MapIcon))
                    MapIcons.Add(item as MapIcon);
            }
            foreach (var item in MapIcons)
            {
                Map.MapElements.Remove(item);
            }
            var nlat = res.Results.Max(x => x.Geometry.Viewport.NorthEast.Latitude);
            var nlng = res.Results.Max(x => x.Geometry.Viewport.NorthEast.Longitude);
            var slat = res.Results.Min(x => x.Geometry.Viewport.SouthWest.Latitude);
            var slng = res.Results.Min(x => x.Geometry.Viewport.SouthWest.Longitude);
            await Map.TrySetViewBoundsAsync(new GeoboundingBox(
                new BasicGeoposition() { Latitude = nlat, Longitude = slng },
                new BasicGeoposition() { Latitude = slat, Longitude = nlng }), null, MapAnimationKind.Bow);
            foreach (var item in res.Results)
            {
                var t = RandomAccessStreamReference.CreateFromUri(new Uri(item.Icon));
                
                //var rt = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, null);
                
                Map.MapElements.Add(new MapIcon()
                {
                    Image = t,
                    Location = new Geopoint(new BasicGeoposition()
                    {
                        Altitude = 0,
                        Latitude = item.Geometry.Location.Latitude,
                        Longitude = item.Geometry.Location.Longitude
                    }),
                    Title = item.Name
                });
            }
        }

        public async void SuggestForSearch(string searchExpression)
        {
            if (searchExpression == "") { SearchResults.Clear(); return; }
            await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                if (SearchResults == null) SearchResults = new ObservableCollection<PlaceAutoComplete.Prediction>();
                SearchResults.Clear();
                SearchResults.Add(new PlaceAutoComplete.Prediction()
                {
                    Description = "Do Nearby Search"
                });
                //Search Saved Places
                var saved = SavedPlacesVM.GetSavedPlaces();
                saved = saved.Where(x => x.PlaceName.Contains(searchExpression)).ToList();
                foreach (var item in saved)
                {
                    SearchResults.Add(new PlaceAutoComplete.Prediction() { PlaceId = $"{item.Latitude},{item.Longitude}", Description = "Saved:" + item.PlaceName });
                }

                //Search in Contacts Address
                if (await SettingsSetters.GetAccessToContcts())
                {
                    var contacts = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                    if (contacts != null)
                    {
                        var search = await contacts.FindContactsAsync(searchExpression);
                        var res = search.Where(x => x.Addresses.Count > 0);
                        foreach (var item in res)
                        {
                            var cadd = item.Addresses.FirstOrDefault();
                            SearchResults.Add(new PlaceAutoComplete.Prediction() { Description = "Contacts:" + item.Name, PlaceId = $"{cadd.StreetAddress}, {cadd.Locality}" });
                        }
                    }
                }

                //Search in API
                if (searchExpression.Length >= 3)
                {
                    var s = await PlaceAutoComplete.GetAutoCompleteResults(searchExpression, location: Map.Center, radius: 50000);
                    if (s == null) return;
                    foreach (var item in s.Predictions)
                    {
                        SearchResults.Add(item);
                    }
                }

            });

        }


        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class SearchUserControlVM : INotifyPropertyChanged
    {

        MapControl _map;

        public event PropertyChangedEventHandler PropertyChanged;

        public MapControl Map { get => _map; set { _map = value; SearchClass.Map = value; } }
        public SearchVMClass SearchClass { get; set; }

        public AppCommand SuggestChoosenCmd { get; set; }
        public AppCommand TextChangedCmd { get; set; }
        public AppCommand QuerySubmittedCmd { get; set; }

        public MapViewVM VM { get; set; }

        public SearchUserControlVM()
        {
            SearchClass = new SearchVMClass();
            SuggestChoosenCmd = AppCommand.GetInstance();
            TextChangedCmd = AppCommand.GetInstance();
            QuerySubmittedCmd = AppCommand.GetInstance();

            SuggestChoosenCmd.ExecuteFunc = SuggestChoosen;
            TextChangedCmd.ExecuteFunc = TextChanged;
            QuerySubmittedCmd.ExecuteFunc = QuerySubmitted;
        }

        private void QuerySubmitted(object obj)
        {
            var searchbox = obj as AutoSuggestBox;
            SearchClass.SearchQuerry = "";
        }

        private void TextChanged(object obj)
        {
            if (!string.IsNullOrEmpty(obj.ToString()))
                SearchClass.SuggestForSearch(obj.ToString());
        }

        private async void SuggestChoosen(object obj)
        {
            var item = obj as PlaceAutoComplete.Prediction;
            if (item.Description == "Do Nearby Search")
            {
                SearchClass.NearbySearchSuggestions();
                return;
            }
            if (item.Description.StartsWith("Saved:"))
            {
                var loc = item.PlaceId.Split(',');
                var lat = loc[0]; var lng = loc[1];
                //VM.UserLocation.Center = new Geopoint(
                //new BasicGeoposition()
                //{
                //    Latitude = Convert.ToDouble(lat),
                //    Longitude = Convert.ToDouble(lng)
                //});
                await VM.Map.TrySetViewAsync(new Geopoint(new BasicGeoposition()
                {
                    Latitude = Convert.ToDouble(lat),
                    Longitude = Convert.ToDouble(lng)
                }), null, null, null, MapAnimationKind.Bow);
                VM.MapRightTapCmd.Execute(new Geopoint(
                    new BasicGeoposition()
                    {
                        Latitude = Convert.ToDouble(lat),
                        Longitude = Convert.ToDouble(lng)
                    }));
                SearchClass.SearchResults.Clear();
                SearchClass.SearchQuerry = "";
                return;
            }
            if (item.Description.StartsWith("Contacts:"))
            {
                var Addres = await PlaceSearchHelper.TextSearch(item.PlaceId);
                if (Addres != null)
                {
                    if (Addres.Results != null && Addres.Results.Any())
                    {
                        var loc = Addres.Results.FirstOrDefault().Geometry.Location;
                        //VM.UserLocation.Center = new Geopoint(
                        //    new BasicGeoposition()
                        //    {
                        //        Latitude = loc.Latitude,
                        //        Longitude = loc.Longitude
                        //    });
                        var bnds = Addres.Results.FirstOrDefault().Geometry.Viewport;

                        await Map.TrySetViewBoundsAsync(new GeoboundingBox(
                            new BasicGeoposition() { Latitude = bnds.NorthEast.Latitude, Longitude = bnds.SouthWest.Longitude },
                            new BasicGeoposition() { Latitude = bnds.SouthWest.Latitude, Longitude = bnds.NorthEast.Longitude }), null, MapAnimationKind.Bow);
                        VM.MapRightTapCmd.Execute(new Geopoint(
                            new BasicGeoposition()
                            {
                                Latitude = loc.Latitude,
                                Longitude = loc.Longitude
                            }));
                        SearchClass.SearchResults.Clear();
                        SearchClass.SearchQuerry = "";
                    }
                }
                return;
            }
            var res = await GeocodeHelper.GetInfo(item.PlaceId);
            SearchClass.SearchQuerry = "";
            if (res == null || res.Results.Length == 0)
            {
                await new MessageDialog(res.Status).ShowAsync();
                return;
            }
            var ploc = res.Results.FirstOrDefault().Geometry.Location;
            //VM.UserLocation.Center = new Geopoint(
            //    new BasicGeoposition()
            //    {
            //        Latitude = ploc.Latitude,
            //        Longitude = ploc.Longitude
            //    });
            var bounds = res.Results.FirstOrDefault().Geometry.Viewport;

            await Map.TrySetViewBoundsAsync(new GeoboundingBox(
                new BasicGeoposition() { Latitude = bounds.NorthEast.Latitude, Longitude = bounds.SouthWest.Longitude },
                new BasicGeoposition() { Latitude = bounds.SouthWest.Latitude, Longitude = bounds.NorthEast.Longitude }), null, MapAnimationKind.Bow);
            VM.MapRightTapCmd.Execute(new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = ploc.Latitude,
                    Longitude = ploc.Longitude
                }));
            SearchClass.SearchResults.Clear();
        }

        void Update(string PName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PName));
        }

    }
}
