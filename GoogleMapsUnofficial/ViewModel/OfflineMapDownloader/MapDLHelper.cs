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
        /// <summary>
        /// Please subscribe this event. When e == true, it means download completed
        /// </summary>
        public EventHandler<bool> DownloadCompleted;
        /// <summary>
        /// This event periodically notify you about the download precent. e == DownloadPercent
        /// </summary>
        public EventHandler<int> DownloadProgress;
        /// <summary>
        /// The event notify you that the download count calculation finished and we started download. e == Number of files to download!
        /// </summary>
        public EventHandler<Int64> DownloadStarted;
        private Int64 _alldls;
        private Int64 _dld;
        private int _perc;
        /// <summary>
        /// All number of files to be downloaded.
        /// </summary>
        public Int64 AllDownloads
        {
            get { return _alldls; }
            set { _alldls = value; }
        }
        public Int64 FailedDownloads { get; set; }
        /// <summary>
        /// Downloaded files count
        /// </summary>
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
        /// <summary>
        /// Download Percentage (Downloaded / All Downloads * 100)
        /// </summary>
        public int DownloadPercent
        {
            get { return _perc; }
            set { _perc = value; DownloadProgress?.Invoke(this, value); }
        }
        private StorageFolder MapFolder { get; set; }
        private const String mapfiles = "http://maps.google.com/mapfiles/mapfiles/132e/map2";
        /// <summary>
        /// Initialize an instance of the class.
        /// </summary>
        public MapDLHelper()
        {
            AsyncInitialize();
            FailedDownloads = 0;
        }
        /// <summary>
        /// Initialize an instance of the class. you can use this method instead of var something = new MapDLHelper();
        /// </summary>
        /// <returns>Instance of the MapDLHelper class.</returns>
        public static MapDLHelper GetInstance()
        {
            return new MapDLHelper();
        }

        /// <summary>
        /// Get map offline storage folder
        /// </summary>
        /// <returns>StorageFolder of offline data</returns>
        public StorageFolder GetMapDownloadFolder()
        {
            return MapFolder;
        }

        private async void AsyncInitialize()
        {
            MapFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MahMaps", CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// Override destination folder of offline map location
        /// </summary>
        /// <param name="OutputFolder"></param>
        /// <returns>acknowledge of override</returns>
        public bool OverrideOutPutFolder(StorageFolder OutputFolder)
        {
            try
            {
                MapFolder = OutputFolder;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Fetch links and save them to file 
        /// </summary>
        /// <param name="href">Tile link to download</param>
        /// <param name="filename">Filename to save on storage</param>
        /// <returns>acknowledge of downloading file</returns>
        private async Task<bool> Download(String href, String filename)
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
                if (buffer.Length == 0) throw new Exception();
                await outp.WriteAsync(buffer);
                buffer.AsStream().Dispose();
                outp.Dispose();
                return true;
            }
            catch
            {
                await file.DeleteAsync();
                FailedDownloads++;
                //ex.Message();
                return false;
            }
        }
        /// <summary>
        /// Download map of a region you mention. We get two points at top left and bottom right
        /// </summary>
        /// <param name="lat_bgn"></param>
        /// <param name="lng_bgn"></param>
        /// <param name="lat_end"></param>
        /// <param name="lng_end"></param>
        /// <param name="MaxZoomLevel">Maximum zoom level to download tiles. Default value is 17</param>
        public async void DownloadMap(double lat_bgn, double lng_bgn, double lat_end, double lng_end, int MaxZoomLevel = 17)
        {
            AllDownloads = 0;
            Downloaded = 0;
            FailedDownloads = 0;
            //Calculate Total downloads number
            for (int z = (int)MapView.MapControl.MinZoomLevel; z <= MaxZoomLevel; z++)
            {
                TileCoordinate c_bgn = new TileCoordinate(lat_bgn, lng_bgn, z);
                var c1 = c_bgn.locationCoord();
                TileCoordinate c_end = new TileCoordinate(lat_end, lng_end, z);
                var c2 = c_end.locationCoord();
                var x_min = (int)c_bgn.x;
                var x_max = (int)c_end.x;

                var y_min = (int)c_bgn.y;
                var y_max = (int)c_end.y;
                for (int x = x_min; x <= x_max; x++)
                {
                    for (int y = y_min; y <= y_max; y++)
                    {
                        AllDownloads++;
                    }
                }
            }

            DownloadStarted?.Invoke(this, AllDownloads);
            //Start Download
            for (int z = (int)MapView.MapControl.MinZoomLevel; z <= MaxZoomLevel; z++)
            {
                TileCoordinate c_bgn = new TileCoordinate(lat_bgn, lng_bgn, z);
                var c1 = c_bgn.locationCoord();
                TileCoordinate c_end = new TileCoordinate(lat_end, lng_end, z);
                var c2 = c_end.locationCoord();
                var x_min = (int)c_bgn.x;
                var x_max = (int)c_end.x;

                var y_min = (int)c_bgn.y;
                var y_max = (int)c_end.y;

                for (int x = x_min; x <= x_max; x++)
                {
                    for (int y = y_min; y <= y_max; y++)
                    {
                        String mapparams = "x_" + x + "-y_" + y + "-z_" + z;
                        //http://mt0.google.com/vt/lyrs=m@405000000&hl=x-local&src=app&sG&x=43614&y=25667&z=16
                        await Download("http://mt" + ((x + y) % 4) + ".google.com/vt/lyrs=m@405000000&hl=x-local&&src=app&sG&x=" + x + "&y=" + y + "&z=" + z, "mah_" + mapparams + ".jpeg");
                        Downloaded++;
                    }
                }

            }

            //Download Completed
            DownloadCompleted?.Invoke(this, true);
            AllDownloads = 0;
        }

    }
}
