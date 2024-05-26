using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Tga;
using TextureCombiner.Source.Datas.Utils;
using TextureCombiner.UI.Controls;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class TgaEncodingOptions : IEncodingOptions
    {
        TgaCompression compressionUsed = TgaCompression.None;

        public TextureFormat GetEncodedFormat() => TextureFormat.TGA;

        public TgaEncodingOptions(EncodingParameters _encodingParameters)
        {
            if (_encodingParameters != null)
                _encodingParameters.OnTGACompressionChanged += SetCompressionUsed;
        }

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            TgaEncoder _encoder = new TgaEncoder();
            _encoder.BitsPerPixel = _useAlpha ? TgaBitsPerPixel.Pixel32 : TgaBitsPerPixel.Pixel24;
            _encoder.Compression = compressionUsed;
            return _encoder;
        }

        void SetCompressionUsed(string _compressionUsedLiteral)
        {
            switch(_compressionUsedLiteral)
            {
                case "None":
                default:
                    compressionUsed = TgaCompression.None;
                    break;
                case "RLE":
                    compressionUsed = TgaCompression.RunLength;
                    break;
            }
        }
    }
}
