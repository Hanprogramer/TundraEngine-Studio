using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.IO;

namespace TundraEngine.Studio.Util
{

    /// <summary>
    /// <para>
    /// Converts a string path to a bitmap asset.
    /// </para>
    /// <para>
    /// The asset must be in the same assembly as the program. If it isn't,
    /// specify "avares://<assemblynamehere>/" in front of the path to the asset.
    /// </para>
    /// </summary>
    public class BitmapAssetValueConverter : IValueConverter
    {
        public static BitmapAssetValueConverter Instance = new BitmapAssetValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is string rawUri && targetType.IsAssignableFrom(typeof(Bitmap)))
            {
                if (rawUri == "" || rawUri.StartsWith("/"))
                    return null;
                Uri uri;

                // Allow for assembly overrides
                if (rawUri.StartsWith("avares://"))
                {
                    uri = new Uri(rawUri);
                    var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    var asset = assets.Open(uri);
                    return new Bitmap(asset);
                }
                else
                {
                    // this is for loading from our own assembly
                    //string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                    //uri = new Uri($"avares://{assemblyName}{rawUri}");
                    //Console.WriteLine("loading " + rawUri);
                    if (File.Exists(rawUri))
                        return new Bitmap(rawUri);
                }


            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}