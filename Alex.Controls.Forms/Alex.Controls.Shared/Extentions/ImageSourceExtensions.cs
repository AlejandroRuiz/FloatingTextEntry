using System;
using Xamarin.Forms;
#if__IOS__
using Xamarin.Forms.Platform.iOS;
#elif __ANDROID__
using Xamarin.Forms.Platform.Android;
#endif

namespace Alex.Controls.Shared.Extentions
{
	internal static class ImageSourceExtensions
	{
		internal static IImageSourceHandler GetHandler (this ImageSource source)
		{
			IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
		}
	}
}

