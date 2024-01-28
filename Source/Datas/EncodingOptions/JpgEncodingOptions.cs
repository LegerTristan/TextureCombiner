using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class JpgEncodingOptions : IEncodingOptions
    {
        public TextureFormat GetEncodedFormat() => TextureFormat.JPG;

        public IImageEncoder GetEncoder()
        {
            JpegEncoder _encoder = new JpegEncoder();
            _encoder.ColorType = JpegColorType.Rgb;
            return _encoder;
        }
    }
}
