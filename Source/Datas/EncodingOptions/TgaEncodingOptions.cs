using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Tga;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class TgaEncodingOptions : IEncodingOptions
    {
        public TextureFormat GetEncodedFormat() => TextureFormat.TGA;

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            TgaEncoder _encoder = new TgaEncoder();
            _encoder.BitsPerPixel = _useAlpha ? TgaBitsPerPixel.Pixel32 : TgaBitsPerPixel.Pixel24;
            return _encoder;
        }
    }
}
