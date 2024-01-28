using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class BmpEncodingOptions : IEncodingOptions
    {
        public TextureFormat GetEncodedFormat() => TextureFormat.BMP;

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            BmpEncoder _encoder = new BmpEncoder();
            _encoder.SupportTransparency = _useAlpha;
            _encoder.BitsPerPixel = _useAlpha ? BmpBitsPerPixel.Pixel32 : BmpBitsPerPixel.Pixel24;
            return _encoder;
        }
    }
}
