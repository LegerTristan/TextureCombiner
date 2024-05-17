using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using TextureCombiner.Source.Datas.Utils;
using TextureCombiner.UI.Controls;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class JpgEncodingOptions : IEncodingOptions
    {
        int quality = 0;

        public TextureFormat GetEncodedFormat() => TextureFormat.JPG;

        public JpgEncodingOptions(EncodingParameters _encodingParameters)
        {
            _encodingParameters.OnQualityUpdated += SetQuality;
        }

        public IImageEncoder GetEncoder()
        {
            JpegEncoder _encoder = new JpegEncoder();
            _encoder.ColorType = JpegColorType.Rgb;
            _encoder.Quality = quality;
            return _encoder;
        }

        void SetQuality(double _value) => quality = (int)_value;
    }
}
