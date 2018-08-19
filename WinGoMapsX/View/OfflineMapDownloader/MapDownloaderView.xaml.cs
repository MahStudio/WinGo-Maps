using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.System.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.ViewModel.OfflineMapDownloader;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinGoMapsX.View.OfflineMapDownloader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapDownloaderView : Page, INotifyPropertyChanged
    {
        bool fp = false;
        bool sp = false;
        private BasicGeoposition _tlp;
        private BasicGeoposition _brp;
        public event PropertyChangedEventHandler PropertyChanged;
        public BasicGeoposition TopLeftPos { get { return _tlp; } set { _tlp = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TopLeftPos")); } }
        public BasicGeoposition BottomRightPos { get { return _brp; } set { _brp = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BottomRightPos")); } }
        MapDLHelper MDH = new MapDLHelper();
        public MapDownloaderView()
        {
            this.InitializeComponent();
            Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r@405000000&hl=x-local&z={zoomlevel}&x={x}&y={y}") { AllowCaching = true }) { IsFadingEnabled = false, AllowOverstretch = true });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Map.Style = MapStyle.None;
            TopLeft.Visibility = Visibility.Collapsed;
            BottomRight.Visibility = Visibility.Collapsed;

            GeoLocatorHelper.GetUserLocation();
            GeoLocatorHelper.LocationFetched += GeoLocatorHelper_LocationFetched;

        }

        private async void GeoLocatorHelper_LocationFetched(object sender, Geoposition e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                await Map.TrySetViewAsync(e.Coordinate.Point, 10, null, null, MapAnimationKind.Bow);
            });
        }

        private async void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                if (fp == false)
                {
                    TopLeft.Visibility = Visibility.Visible;
                    TopLeftPos = args.Location.Position;
                    MapControl.SetLocation(TopLeft, args.Location);
                    fp = true;
                }
                else
                {
                    if (sp == false)
                    {
                        BottomRight.Visibility = Visibility.Visible;
                        BottomRightPos = args.Location.Position;
                        MapControl.SetLocation(BottomRight, args.Location);
                        sp = true;
                        DLButton.IsEnabled = true;
                    }
                    else
                    {
                        await new MessageDialog(MultilingualHelpToolkit.GetString("MapDownloaderViewStringTwoPointsSelected", "Text")).ShowAsync();
                    }
                }
            });
        }

        private void DownloadMap_Click(object sender, RoutedEventArgs e)
        {
            DLButton.IsEnabled = false;
            MDH.DownloadProgress += new EventHandler<int>(DLProgress);
            MDH.DownloadCompleted += new EventHandler<bool>(DLComplete);
            MDH.DownloadMap(TopLeftPos.Latitude, TopLeftPos.Longitude,
                                BottomRightPos.Latitude, BottomRightPos.Longitude, (int)MaxZoom.Value);
        }

        private async void DLComplete(object sender, bool e)
        {
            if (MDH != null)
            {
                try
                {
                    MDH.DownloadCompleted -= DLComplete;
                    MDH.DownloadProgress -= DLProgress;

                    if (MDH.FailedDownloads == 0)
                        await new MessageDialog(MultilingualHelpToolkit.GetString("MapDownloaderViewStringDLComplete", "Text")).ShowAsync();
                    else
                        await new MessageDialog(MultilingualHelpToolkit.GetString("MapDownloaderViewStringDLCompleteWithErrors", "Text").Replace("{count}", $"{MDH.FailedDownloads}")).ShowAsync();
                }
                catch { }
            }
            try
            {
                DLButton.IsEnabled = true;
            }
            catch { }
            try
            {
                new DisplayRequest().RequestRelease();
            }
            catch { }
        }

        private void DLProgress(object sender, int e)
        {
            DLPB.Value = e;
            try
            {
                new DisplayRequest().RequestActive();
            }
            catch
            {
            }
        }

        private async void BackupBTN_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker fop = new FolderPicker() { ViewMode = PickerViewMode.List, SuggestedStartLocation = PickerLocationId.Downloads };
            fop.FileTypeFilter.Add(".png");
            fop.FileTypeFilter.Add(".jpg");
            var fol = await fop.PickSingleFolderAsync();
            if (fol != null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    var OfflineMap = MDH.GetMapDownloadFolder();
                    var files = await OfflineMap.GetFilesAsync();
                    var Count = files.Count;
                    var Counter = 0;
                    foreach (var item in files)
                    {
                        try
                        {
                            await item.CopyAsync(fol);
                            Counter++;
                            DLPB.Value = (((float)Counter / (float)Count) * 100);
                        }
                        catch { }
                    }
                });
            }
        }

        private async void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker fop = new FolderPicker() { ViewMode = PickerViewMode.List, SuggestedStartLocation = PickerLocationId.Downloads };
            fop.FileTypeFilter.Add(".png");
            fop.FileTypeFilter.Add(".jpg");
            var fol = await fop.PickSingleFolderAsync();
            if (fol != null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    var OfflineMap = MDH.GetMapDownloadFolder();
                    var files = await fol.GetFilesAsync();
                    var Count = files.Count;
                    var Counter = 0;
                    foreach (var item in files)
                    {
                        try
                        {
                            Counter++;
                            DLPB.Value = (((float)Counter / (float)Count) * 100);
                            await item.CopyAsync(OfflineMap);
                        }
                        catch { }
                    }
                });
            }
        }

        private void BottomRight_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            BottomRight.Visibility = Visibility.Collapsed;
            sp = false;
            DLButton.IsEnabled = false;
        }

        private void TopLeft_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            TopLeft.Visibility = Visibility.Collapsed;
            fp = false;
            DLButton.IsEnabled = false;
        }
    }
}
