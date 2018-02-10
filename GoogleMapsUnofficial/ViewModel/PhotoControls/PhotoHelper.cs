using System;
using Windows.UI.Xaml.Media.Imaging;

namespace GoogleMapsUnofficial.ViewModel.PhotoControls
{
    class PhotosHelper
    {
        /// <summary>
        /// Get a photo from photo reference id
        /// </summary>
        /// <param name="PhotoReference">photo reference id get from Google Maps api</param>
        /// <param name="MaxWidth">maximum width of the picture</param>
        /// <param name="MaxHeight">maximum height of the picture</param>
        /// <returns>a link that return you the image uri</returns>
        public static Uri GetPhotoUri(string PhotoReference, int MaxWidth, int MaxHeight)
        {
            return new Uri($"https://maps.googleapis.com/maps/api/place/photo?key={AppCore.GoogleMapAPIKey}&photoreference={PhotoReference}&maxwidth={MaxWidth}&maxheight={MaxHeight}");
        }
        /// <summary>
        /// Get a photo from photo reference id
        /// </summary>
        /// <param name="PhotoReference">photo reference id get from Google Maps api</param>
        /// <param name="MaxWidth">maximum width of the picture</param>
        /// <param name="MaxHeight">maximum height of the picture</param>
        /// <returns>photo as a BitmapImage class</returns>
        public static BitmapImage GetPhoto(string PhotoReference, int MaxWidth, int MaxHeight)
        {
            var uri = new Uri($"https://maps.googleapis.com/maps/api/place/photo?key={AppCore.GoogleMapAPIKey}&photoreference={PhotoReference}&maxwidth={MaxWidth}&maxheight={MaxHeight}");
            return new BitmapImage(uri);
        }
    }
}
