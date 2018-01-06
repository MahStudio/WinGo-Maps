using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Storage.Streams;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using GoogleMapsUnofficial.View;

namespace GoogleMapsUnofficial.ViewModel.OfflineMapDownloader
{
    public class MapDLHelper
    {
        public EventHandler<bool> DownloadCompleted;
        public EventHandler<int> DownloadProgress;
        private Int64 _alldls;
        private Int64 _dld;
        private int _perc;
        public Int64 AllDownloads
        {
            get { return _alldls; }
            set { _alldls = value; }
        }
        public Int64 Downloaded
        {
            get { return _dld; }
            set
            {
                _dld = value; try
                {
                    var p = (((float)value / (float)AllDownloads) * 100);
                    DownloadPercent = Convert.ToInt32(p);
                }
                catch { }
            }
        }
        public int DownloadPercent
        {
            get { return _perc; }
            set { _perc = value; DownloadProgress?.Invoke(this, value); }
        }
        private StorageFolder MapFolder { get; set; }
        private const String mapfiles = "http://maps.google.com/mapfiles/mapfiles/132e/map2";
        public MapDLHelper()
        {
            AsyncInitialize();
        }

        async void AsyncInitialize()
        {
            MapFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MahMaps", CreationCollisionOption.OpenIfExists);
        }

        public async Task<bool> Download(String href, String filename)
        {
            //mkdir if folder not existed
            StorageFile file = null;
            try
            {
                file = await MapFolder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
            }
            catch { /* Already Downloaded */ return true; }

            IRandomAccessStream outp = null;
            try
            {
                var url = new Uri(href);
                outp = (await file.OpenAsync(FileAccessMode.ReadWrite));
                var http = new HttpClient();
                http.DefaultRequestHeaders.Accept.ParseAdd("text/html, application/xhtml+xml, image/jxr, */*");
                http.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.7,fa;q=0.3");
                http.DefaultRequestHeaders.Cookie.ParseAdd($"IP_JAR={DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-21");
                http.DefaultRequestHeaders.UserAgent.ParseAdd($"{AppCore.HttpUserAgent}");
                var res = await http.GetAsync(url);
                var buffer = await res.Content.ReadAsBufferAsync();
                await outp.WriteAsync(buffer);
                buffer.AsStream().Dispose();
                outp.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                //ex.Message();
                return false;
            }
        }

        public async void DownloadMap(double lat_bgn, double lng_bgn, double lat_end, double lng_end)
        {
            Downloaded = 0;
            for (int z = (int)MapView.MapControl.MinZoomLevel; z <= (int)MapView.MapControl.MaxZoomLevel; z++)
            {
                TileCoordinate c_bgn = new TileCoordinate(lat_bgn, lng_bgn, z);
                var c1 = c_bgn.locationCoord();
                TileCoordinate c_end = new TileCoordinate(lat_end, lng_end, z);
                var c2 = c_end.locationCoord();
                var x_min = (int)c_bgn.x;
                var x_max = (int)c_end.x;

                var y_min = (int)c_bgn.y;
                var y_max = (int)c_end.y;
                //Calculate download
                AllDownloads = (x_max - x_min + 1) * (y_max - y_min + 1);
                Downloaded = 0;

                for (int x = x_min; x <= x_max; x++)
                {
                    for (int y = y_min; y <= y_max; y++)
                    {
                        String mapparams = "x_" + x + "-y_" + y + "-z_" + z;
                        //http://mt0.google.com/vt/lyrs=m@405000000&hl=x-local&src=app&sG&x=43614&y=25667&z=16
                        await Download("http://mt" + ((x + y) % 4) + ".google.com/vt/lyrs=m@405000000&hl=x-local&&src=app&sG&x=" + x + "&y=" + y + "&z=" + z, "mah_" + mapparams + ".jpeg");
                        Downloaded++;
                        if (z == (int)MapView.MapControl.MaxZoomLevel)
                            if (x == x_max)
                                if (y == y_max)
                                    DownloadCompleted?.Invoke(this, true);
                    }
                }

            }
        }

    }
}
