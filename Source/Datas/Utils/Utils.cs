using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace TextureCombiner.Source.Datas.Utils
{
    public static class Utils
    {
        public static BitmapImage LoadBitmapImage(string _imagePath)
        {
            BitmapImage _bitmapImage = new BitmapImage();
            byte[] _imageData = File.ReadAllBytes(_imagePath);
            using (var _mem = new MemoryStream(_imageData))
            {
                _mem.Position = 0;
                _bitmapImage.BeginInit();
                _bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                _bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                _bitmapImage.UriSource = null;
                _bitmapImage.StreamSource = _mem;
                _bitmapImage.EndInit();

            }
            return _bitmapImage;
        }

        public static int GetBitmapStride(BitmapSource _src)
        {
            return _src != null ? ((_src.PixelWidth * _src.Format.BitsPerPixel + 31) >> 5) << 2 : -1;
        }
    }
}
