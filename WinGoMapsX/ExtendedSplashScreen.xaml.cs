using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.ViewModel.PlacesControls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinGoMapsX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen : Page
    {
        DispatcherTimer DispatcherTime;
        object para = null;
        public ExtendedSplashScreen(SplashScreen splash, object parameter = null)
        {
            this.InitializeComponent();
            this.Loaded += ExtendedSplashScreen_Loaded;
            para = parameter;
            DispatcherTime = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 20)
            };
            DispatcherTime.Tick += DispatcherTime_Tick;
            DispatcherTime.Start();
        }

        private async void DispatcherTime_Tick(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, RemoveExtendedSplash);
        }
        void LoadExtendedSplashScreen()
        {
            InstallVCD();
            UpdateJumpList();
            RemoveExtendedSplash();
        }
        public async void InstallVCD()
        {
            try
            {
                var r = ApplicationData.Current.LocalSettings.Values["VCDV10"].ToString();
            }
            catch (Exception)
            {
                try
                {
                    var vcdStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommands.xml", UriKind.RelativeOrAbsolute));
                    await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
                    ApplicationData.Current.LocalSettings.Values["VCDV10"] = "";
                }
                catch
                {
                    ApplicationData.Current.LocalSettings.Values["VCDV10"] = null;
                }
            }
        }
        public async void UpdateJumpList()
        {
            try
            {
                var listjump = await JumpList.LoadCurrentAsync();
                listjump.Items.Clear();
                foreach (var Place in SavedPlacesVM.GetSavedPlaces())
                {
                    listjump.SystemGroupKind = JumpListSystemGroupKind.None;
                    listjump.Items.Add(JumpListItem.CreateWithArguments($"{Place.Latitude},{Place.Longitude}", Place.PlaceName));
                }
                await listjump.SaveAsync();
            }
            catch { }
        }
        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, LoadExtendedSplashScreen);
        }
        void RemoveExtendedSplash()
        {
            try
            {
                DispatcherTime.Stop();
                DispatcherTime.Tick -= DispatcherTime_Tick;
                DispatcherTime = null;
                Window.Current.Content = new MainPage(para);
                Window.Current.Activate();
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
