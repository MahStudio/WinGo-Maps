using GoogleMapsUnofficial.ViewModel.OfflineMapDownloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoogleMapsUnofficial.View.OfflineMapDownloader
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
            Map.Style = MapStyle.None;
            TopLeft.Visibility = Visibility.Collapsed;
            BottomRight.Visibility = Visibility.Collapsed;
            Map.TileSources.Add(new MapTileSource(new HttpMapTileDataSource("http://mt1.google.com/vt/lyrs=r@405000000&hl=x-local&z={zoomlevel}&x={x}&y={y}") { AllowCaching = true }));
            this.Loaded += MapDownloaderView_Loaded;
        }

        private async void MapDownloaderView_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
            {
                await Task.Delay(20);
                if (MDH.GetMapDownloadFolder() == null)
                {
                    BackupBTN.IsEnabled = false;
                }
            });
        }

        private async void Map_MapTapped(MapControl sender, MapInputEventArgs args)
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
                if(sp == false)
                {
                    BottomRight.Visibility = Visibility.Visible;
                    BottomRightPos = args.Location.Position;
                    MapControl.SetLocation(BottomRight, args.Location);
                    sp = true;
                    DLButton.IsEnabled = true;
                }
                else
                {
                    await new MessageDialog("You selected 2 points.").ShowAsync();
                }
            }

        }

        private void DownloadMap_Click(object sender, RoutedEventArgs e)
        {
            MDH.DownloadProgress += new EventHandler<int>(DLProgress);
            MDH.DownloadCompleted += new EventHandler<bool>(DLComplete);
            MDH.DownloadMap(TopLeftPos.Latitude, TopLeftPos.Longitude,
                                BottomRightPos.Latitude, BottomRightPos.Longitude, (int)MaxZoom.Value);
        }

        private async void DLComplete(object sender, bool e)
        {
            await new MessageDialog("Download Completed.").ShowAsync();
        }

        private void DLProgress(object sender, int e)
        {
            DLPB.Value = e;
        }

        private async void BackupBTN_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker fop = new FolderPicker() { ViewMode = PickerViewMode.List, SuggestedStartLocation = PickerLocationId.Downloads };
            fop.FileTypeFilter.Add(".png");
            fop.FileTypeFilter.Add(".jpg");
            var fol = await fop.PickSingleFolderAsync();
            if(fol != null)
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
            }
        }
    }
}
