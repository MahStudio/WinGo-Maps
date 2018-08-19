using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinGoMapsX.View.OnMapControls
{
    public sealed partial class MyLocationUserControl : UserControl
    {
        int count = 0;
        public MapViewVM MapViewVM { get; set; }
        public MyLocationUserControl()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await AppCore.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, RunBTNClick);
        }

        private async void RunBTNClick()
        {
            count++;
            await Task.Delay(350);
            if (count == 0) return;
            if (count == 1)
            {
                try
                {
                    await Task.Delay(10);
                    GeoLocatorHelper.GetUserLocation();
                }
                catch { }
            }
            else
            {
                try
                {
                    if (Compass.GetDefault() == null) return;
                    if (thisbtn.Content.ToString() == "")
                    {
                        MapViewVM.HeadingLocIndicatorVisibility = Visibility.Visible;
                        thisbtn.Content = "";
                    }
                    else
                    {
                        MapViewVM.HeadingLocIndicatorVisibility = Visibility.Collapsed;
                        thisbtn.Content = "";
                    }
                    //if (MapViewVM.Compass != null)
                    //{
                    //    MapViewVM.CompassEnabled = !MapViewVM.CompassEnabled;
                    //    if (MapViewVM.CompassEnabled)
                    //        thisbtn.Content = "";
                    //    else thisbtn.Content = "";
                    //}
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            }
            count = 0;
        }
    }
}
