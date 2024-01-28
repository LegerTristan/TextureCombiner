using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class PngEncodingOptions : IEncodingOptions
    {
        public TextureFormat GetEncodedFormat() => TextureFormat.PNG;

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            PngEncoder _encoder = new PngEncoder();
            _encoder.ColorType = _useAlpha ? PngColorType.RgbWithAlpha : PngColorType.Rgb;
            _encoder.BitDepth = PngBitDepth.Bit8;
            return _encoder;
        }
    }
}
