using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace TextureCombiner.Source.Datas.Utils
{
    public static class BitmapSourceExtension
    {
        public static byte[] GetPixels(this BitmapSource _src)
        {
            int _stride = Utils.GetBitmapStride(_src);
            byte[] _pixels = new byte[_stride * _src.PixelHeight];
            _src.CopyPixels(_pixels, _stride, 0);
            return _pixels;
        }

        public static Image ToImageSharp<TPixelFormat>(this BitmapSource _src) where TPixelFormat : unmanaged, IPixel<TPixelFormat>
        {
            return Image.LoadPixelData<TPixelFormat>(_src.GetPixels(), _src.PixelWidth, _src.PixelHeight);
        }
    }
}
