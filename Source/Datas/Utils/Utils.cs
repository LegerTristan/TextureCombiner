using System;
using System.Windows.Media.Imaging;

namespace TextureCombiner.Source.Datas.Utils
{
    public static class Utils
    {
        public static BitmapImage LoadBitmapImage(string _imagePath)
        {
            Uri _uri = new Uri(_imagePath, UriKind.RelativeOrAbsolute);
            BitmapImage _bitmapImage = new BitmapImage(_uri);
            return _bitmapImage;
        }

        public static int GetBitmapStride(BitmapSource _src)
        {
            return ((_src.PixelWidth * _src.Format.BitsPerPixel + 31) >> 5) << 2;
        }
    }
}
