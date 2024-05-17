using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using TextureCombiner.Source.Datas.Utils;
using TextureCombiner.UI.Controls;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class TiffEncodingOptions : IEncodingOptions
    {
        TiffCompression compressionUsed = TiffCompression.None;

        int compressionLevel = 0;

        public TextureFormat GetEncodedFormat() => TextureFormat.TIFF;

        public TiffEncodingOptions(EncodingParameters _encodingParameters)
        {
            _encodingParameters.OnCompressionLevelUpdated += SetCompressionLevel;
            _encodingParameters.OnTIFFCompressionChanged += SetCompressionUsed;
        }

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            TiffEncoder _encoder = new TiffEncoder();
            _encoder.BitsPerPixel = TiffBitsPerPixel.Bit24;
            _encoder.Compression = compressionUsed;
            _encoder.CompressionLevel = GetCompressionLevel();
            return _encoder;
        }

        void SetCompressionLevel(double _value) => compressionLevel = (int)_value;

        DeflateCompressionLevel GetCompressionLevel()
        {
            switch (compressionLevel)
            {
                case 0:
                default:
                    return DeflateCompressionLevel.Level0;
                case 1:
                    return DeflateCompressionLevel.Level1;
                case 2:
                    return DeflateCompressionLevel.Level2;
                case 3:
                    return DeflateCompressionLevel.Level3;
                case 4:
                    return DeflateCompressionLevel.Level4;
                case 5:                    
                    return DeflateCompressionLevel.Level5;
                case 6:
                    return DeflateCompressionLevel.Level6;
                case 7:
                    return DeflateCompressionLevel.Level7;
                case 8:
                    return DeflateCompressionLevel.Level8;
                case 9:
                    return DeflateCompressionLevel.Level9;
            }
        }

        void SetCompressionUsed(string _compressionUsedLiteral)
        {
            switch (_compressionUsedLiteral)
            {
                case "None":
                default:
                    compressionUsed = TiffCompression.None;
                    break;
                case "LZW":
                    compressionUsed = TiffCompression.Lzw;
                    break;
                case "PackBits":
                    compressionUsed = TiffCompression.PackBits;
                    break;
                case "Deflate":
                    compressionUsed = TiffCompression.Deflate;
                    break;
            }
        }
    }
}
