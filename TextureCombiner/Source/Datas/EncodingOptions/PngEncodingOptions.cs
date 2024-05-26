using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using TextureCombiner.Source.Datas.Utils;
using TextureCombiner.UI.Controls;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class PngEncodingOptions : IEncodingOptions
    {
        int compressionLevel = 0;

        public TextureFormat GetEncodedFormat() => TextureFormat.PNG;

        public PngEncodingOptions(EncodingParameters _encodingParameters)
        {
            if (_encodingParameters != null)
                _encodingParameters.OnCompressionLevelUpdated += SetCompressionLevel;
        }

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            PngEncoder _encoder = new PngEncoder();
            _encoder.ColorType = _useAlpha ? PngColorType.RgbWithAlpha : PngColorType.Rgb;
            _encoder.BitDepth = PngBitDepth.Bit8;
            _encoder.CompressionLevel = GetCompressionLevel();
            return _encoder;
        }

        void SetCompressionLevel(double _value) => compressionLevel = (int)_value;

        PngCompressionLevel GetCompressionLevel()
        {
            switch(compressionLevel)
            {
                case 0:
                default:
                    return PngCompressionLevel.Level0;
                case 1:
                    return PngCompressionLevel.Level1;
                case 2:
                    return PngCompressionLevel.Level2;
                case 3:
                    return PngCompressionLevel.Level3;
                case 4:
                    return PngCompressionLevel.Level4;
                case 5:
                    return PngCompressionLevel.Level5;
                case 6:
                    return PngCompressionLevel.Level6;
                case 7:
                    return PngCompressionLevel.Level7;
                case 8:
                    return PngCompressionLevel.Level8;
                case 9:
                    return PngCompressionLevel.Level9;
            }
        }
    }
}
