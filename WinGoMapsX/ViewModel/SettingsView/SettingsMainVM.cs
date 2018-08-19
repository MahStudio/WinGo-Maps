using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Contacts;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls.Maps;

namespace WinGoMapsX.ViewModel.SettingsView
{
    class SettingsMainVM : INotifyPropertyChanged
    {
        private bool _contactsaccess;
        private bool _showtrafficonlaunch;
        private int _themeindex = 0;
        private int _lengthUnitindex = 0;
        private int _rotationcontrolsVisible = -1;
        private int _zoomcontrolsVisible = -1;
        private bool _fadeanimationEnabled;
        private bool _allowOverstretch;
        private bool _livetileenable;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool ContactsAccess
        {
            get => _contactsaccess;
            set
            {
                UpdateContactsAccess(value);
            }
        }
        public async void UpdateContactsAccess(bool V)
        {
            _contactsaccess = await SettingsSetters.SetAccessToContacts(V);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactsAccess"));
        }
        public bool ShowTrafficOnLaunch
        {
            get => _showtrafficonlaunch;
            set
            {
                _showtrafficonlaunch = value;
                SettingsSetters.SetShowTrafficOnLaunch(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowTrafficOnLaunch"));
            }
        }
        public bool LiveTileEnable
        {
            get => _livetileenable;
            set
            {
                _livetileenable = value;
                SettingsSetters.SetLiveTileBackgroundTask(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LiveTileEnable"));
            }
        }
        public bool AllowOverstretch
        {
            get => _allowOverstretch;
            set
            {
                _allowOverstretch = value;
                SettingsSetters.SetAllowOverstretch(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowOverstretch"));
            }
        }
        public bool FadeAnimationEnabled
        {
            get => _fadeanimationEnabled; 
            set
            {
                _fadeanimationEnabled = value;
                SettingsSetters.SetFadeAnimationEnabled(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FadeAnimationEnabled"));
            }
        }
        public int ZoomControlsVisible
        {
            get => _zoomcontrolsVisible; 
            set
            {
                _zoomcontrolsVisible = value;
                if (_zoomcontrolsVisible != -1)
                {
                    SettingsSetters.SetZoomControlsVisible(StringToEnumConverter(MapInteractionModeOptions[value]));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomControlsVisible"));
            }
        }
        public int RotationControlsVisible
        {
            get => _rotationcontrolsVisible; 
            set
            {
                _rotationcontrolsVisible = value;
                if (_rotationcontrolsVisible != -1)
                {
                    SettingsSetters.SetRotationControlsVisible(StringToEnumConverter(MapInteractionModeOptions[value]));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RotationControlsVisible"));
            }
        }
        public int LengthUnit
        {
            get => _lengthUnitindex; 
            set
            {
                _lengthUnitindex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LengthUnit"));
                SettingsSetters.SetLengthUnit(value);
            }
        }
        public int ThemeIndex
        {
            get => _themeindex; 
            set
            {
                _themeindex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThemeIndex"));
                SettingsSetters.SetThemeIndex(value);
            }
        }
        public List<string> MapInteractionModeOptions
        {
            get => Enum.GetNames(typeof(MapInteractionMode)).ToList(); 
        }
        public List<string> LengthUnits
        {
            get => new List<string>() { $"{MultilingualHelpToolkit.GetString("StringMetricLengthUnit", "Text")}", $"{MultilingualHelpToolkit.GetString("StringImperialLengthUnit", "Text")}", $"{MultilingualHelpToolkit.GetString("StringUSLengthUnit", "Text")}" }; 
        }
        public List<string> AvailableThemes
        {
            get => new List<string>() { $"{MultilingualHelpToolkit.GetString("StringDarkTheme", "Text")}", $"{MultilingualHelpToolkit.GetString("StringLightTheme", "Text")}", $"{MultilingualHelpToolkit.GetString("StringSystemTheme", "Text")}" }; 
        }
        public static List<string> MapInteractionModeOptionsstatic
        {
            get => Enum.GetNames(typeof(MapInteractionMode)).ToList(); 
        }
        public string ApplicationVersion
        {
            get { var ver = Package.Current.Id.Version; return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}"; }
        }
        public SettingsMainVM()
        {
            AllowOverstretch = SettingsSetters.GetAllowOverstretch();
            FadeAnimationEnabled = SettingsSetters.GetFadeAnimationEnabled();
            ShowTrafficOnLaunch = SettingsSetters.GetShowTrafficOnLaunch();
            ZoomControlsVisible = EnumToIndexConverter(SettingsSetters.GetZoomControlsVisible());
            RotationControlsVisible = EnumToIndexConverter(SettingsSetters.GetRotationControlsVisible());
            LengthUnit = SettingsSetters.GetLengthUnit();
            ThemeIndex = SettingsSetters.GetThemeIndex();
            BackgroundHandler();
            ContactHandler();
        }
        async void ContactHandler() => ContactsAccess = await SettingsSetters.GetAccessToContcts();
        
        async void BackgroundHandler()
        {
            try
            {
                var req = await BackgroundExecutionManager.RequestAccessAsync();
                if (req == BackgroundAccessStatus.AlwaysAllowed || req == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                         req == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity || req == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    var list = BackgroundTaskRegistration.AllTasks.Where(x => x.Value.Name == "WinGoMapsTile");
                    if (list.Count() != 0) LiveTileEnable = true;
                    else LiveTileEnable = false;
                }
            }
            catch { LiveTileEnable = false; }
        }

        public static int EnumToIndexConverter(MapInteractionMode Entry) =>  MapInteractionModeOptionsstatic.FindIndex(x => x == Entry.ToString());

        public static MapInteractionMode StringToEnumConverter(string option) => (MapInteractionMode)Enum.Parse(typeof(MapInteractionMode), option);
        
    }

    class SettingsSetters
    {
        public static bool GetShowTrafficOnLaunch()
        {
            try
            {
                return (bool)ApplicationData.Current.LocalSettings.Values["ShowTrafficOnLaunch"];
            }
            catch
            {
                SetShowTrafficOnLaunch(false);
                return false;
            }
        }

        public static void SetShowTrafficOnLaunch(bool Value) => ApplicationData.Current.LocalSettings.Values["ShowTrafficOnLaunch"] = Value;

        public static bool GetFadeAnimationEnabled()
        {
            try
            {
                return (bool)ApplicationData.Current.LocalSettings.Values["MapFadeAnimationEnabled"];
            }
            catch
            {
                SetFadeAnimationEnabled(false);
                return true;
            }
        }

        public static void SetFadeAnimationEnabled(bool Value) => ApplicationData.Current.LocalSettings.Values["MapFadeAnimationEnabled"] = Value;

        public static bool GetAllowOverstretch()
        {
            try
            {
                return (bool)ApplicationData.Current.LocalSettings.Values["AllowOverstretch"];
            }
            catch
            {
                SetAllowOverstretch(true);
                return true;
            }
        }

        public static void SetAllowOverstretch(bool Value) => ApplicationData.Current.LocalSettings.Values["AllowOverstretch"] = Value;

        public static MapInteractionMode GetZoomControlsVisible()
        {
            try
            {
                var r = ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible2"].ToString();
                return SettingsMainVM.StringToEnumConverter(r);
                //return (MapInteractionMode)ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible"];
            }
            catch
            {
                SetZoomControlsVisible(MapInteractionMode.GestureAndControl);
                return MapInteractionMode.GestureAndControl;
            }
        }

        public static void SetZoomControlsVisible(MapInteractionMode Value) => ApplicationData.Current.LocalSettings.Values["ZoomControlsVisible2"] = Value.ToString();

        public static MapInteractionMode GetRotationControlsVisible()
        {
            try
            {
                var r = ApplicationData.Current.LocalSettings.Values["RotationControlsVisible2"].ToString();
                return SettingsMainVM.StringToEnumConverter(r);
                //return (MapInteractionMode)ApplicationData.Current.LocalSettings.Values["RotationControlsVisible"];
            }
            catch
            {
                if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone)
                {
                    SetRotationControlsVisible(MapInteractionMode.GestureOnly);
                    return MapInteractionMode.GestureAndControl;
                }
                else
                {
                    SetRotationControlsVisible(MapInteractionMode.GestureAndControl);
                    return MapInteractionMode.GestureAndControl;
                }
            }
        }

        public static void SetRotationControlsVisible(MapInteractionMode Value) => ApplicationData.Current.LocalSettings.Values["RotationControlsVisible2"] = Value.ToString();

        public static async void SetLiveTileBackgroundTask(bool Val)
        {
            try
            {
                var req = await BackgroundExecutionManager.RequestAccessAsync();
                if (req == BackgroundAccessStatus.AlwaysAllowed || req == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                         req == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity || req == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    var list = BackgroundTaskRegistration.AllTasks.Where(x => x.Value.Name == "WinGoMapsTile");
                    foreach (var item in list)
                    {
                        item.Value.Unregister(false);
                    }
                    if (Val)
                    {
                        BackgroundTaskBuilder build = new BackgroundTaskBuilder();
                        build.IsNetworkRequested = true;
                        build.Name = "WinGoMapsTile";
                        build.TaskEntryPoint = "LiveTileTask.Update";
                        build.SetTrigger(new TimeTrigger(15, false));
                        var b = build.Register();
                        if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
                        {
                            var geolocator = new Geolocator();
                            var Location = await geolocator.GetGeopositionAsync();
                            var ul = Location.Coordinate.Point.Position;
                            var http = AppCore.HttpClient;
                            http.DefaultRequestHeaders.UserAgent.ParseAdd("MahStudioWinGoMaps");
                            var res = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=200x200", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                            var reswide = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=310x150", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                            var resLarge = await (await http.GetAsync(new Uri($"https://maps.googleapis.com/maps/api/staticmap?center={ul.Latitude},{ul.Longitude}&zoom=16&size=310x310", UriKind.RelativeOrAbsolute))).Content.ReadAsBufferAsync();
                            var f = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTile.png", CreationCollisionOption.OpenIfExists);
                            var fWide = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTileWide.png", CreationCollisionOption.OpenIfExists);
                            var fLarge = await ApplicationData.Current.LocalFolder.CreateFileAsync("LiveTileLarge.png", CreationCollisionOption.OpenIfExists);
                            var str = await f.OpenAsync(FileAccessMode.ReadWrite);
                            var strwide = await fWide.OpenAsync(FileAccessMode.ReadWrite);
                            var strLarge = await fLarge.OpenAsync(FileAccessMode.ReadWrite);
                            await str.WriteAsync(res);
                            await strwide.WriteAsync(reswide);
                            await strLarge.WriteAsync(resLarge);
                            str.Dispose();
                            strwide.Dispose();
                            strLarge.Dispose();

                            var update = TileUpdateManager.CreateTileUpdaterForApplication();
                            string xml = "<tile>\n";
                            xml += "<visual version=\"4\">\n";
                            xml += "  <binding template=\"TileSquare150x150Image\" fallback=\"TileSquareImage\">\n";
                            xml += "      <image id=\"1\" src=\"ms-appdata:///local/LiveTile.png\"/>\n";
                            xml += "  </binding>\n";
                            xml += "  <binding template=\"TileWide310x150Image\" fallback=\"TileWideImage\">\n";
                            xml += "      <image id=\"1\" src=\"ms-appdata:///local/LiveTileWide.png\"/>\n";
                            xml += "  </binding>\n";
                            xml += "  <binding template=\"TileSquare310x310Image\" fallback=\"TileSquareImageLarge\">\n";
                            xml += "      <image id=\"1\" src=\"ms-appdata:///local/LiveTileLarge.png\"/>\n";
                            xml += "  </binding>\n";
                            xml += "</visual>\n";
                            xml += "</tile>";

                            XmlDocument txml = new XmlDocument();
                            txml.LoadXml(xml);
                            TileNotification tNotification = new TileNotification(txml);

                            update.Clear();
                            update.Update(tNotification);
                            //XmlDocument tileXmlwide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Image);
                            //new Windows.UI.Popups.MessageDialog(tileXmlwide.GetXml()).ShowAsync();
                            //XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);
                            //tileXml.GetElementsByTagName("image")[0].Attributes[1].NodeValue = "ms-appdata:///local/LiveTile.png";
                            //tileXmlwide.GetElementsByTagName("image")[0].Attributes[1].NodeValue = "ms-appdata:///local/LiveTileWide.png";
                            //update.Update(new TileNotification(xml));
                        }
                    }
                }
            }
            catch { }
        }

        public static int GetLengthUnit()
        {
            try
            {
                return (int)ApplicationData.Current.LocalSettings.Values["LengthUnit"];
            }
            catch (Exception)
            {
                SetLengthUnit(0);
                return 0;
            }
        }

        public static void SetLengthUnit(int Index) => ApplicationData.Current.LocalSettings.Values["LengthUnit"] = Index;

        public static int GetThemeIndex()
        {
            try
            {
                return (int)ApplicationData.Current.LocalSettings.Values["ThemeIndex"];
            }
            catch (Exception)
            {
                SetThemeIndex(1);
                return 1;
            }
        }

        public static void SetThemeIndex(int Index) => ApplicationData.Current.LocalSettings.Values["ThemeIndex"] = Index;
        
        public static async Task<bool> GetAccessToContcts()
        {
            try
            {
                var data = Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values["AccessContacts"]);
                if (data == false) return false;
                var at = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                if (at != null)
                    return true;
                else return false;
            }
            catch
            {
                return await SetAccessToContacts(false);
            }
        }

        public static async Task<bool> SetAccessToContacts(bool Value)
        {
            try
            {
                if (Value == false)
                {
                    ApplicationData.Current.LocalSettings.Values["AccessContacts"] = false;
                    return false;
                }
                var at = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                if (at != null)
                {
                    ApplicationData.Current.LocalSettings.Values["AccessContacts"] = true;
                    return true;
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["AccessContacts"] = false;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
