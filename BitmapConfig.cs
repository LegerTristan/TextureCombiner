using System.Drawing.Imaging;

namespace TextureCombiner
{
    /// <summary>
    /// Format that can hanle the program
    /// </summary>
    public enum TextureFormat
    {
        JPG,
        PNG,
        TGA,
        TIFF,
        WEBP
    }

    /// <summary>
    /// Parameters to config a bitmap such as format, size, and use base texture's path.
    /// </summary>
    public struct BitmapConfig
    {
        #region Constants
        public const int MIN_SIZE = 1, MAX_SIZE = 4096;
        #endregion

        #region F/P
        public string[] TexturePaths { get; set; }
        public TextureFormat Format { get; set; }
        public EncoderValue CompressionType { get; set; }
        public int CompressQuality { get; set; }
        public bool UseAlpha { get; set; }
        public bool UseCompression { get; set; }

        public bool IsLossyFormat => Format == TextureFormat.JPG || Format == TextureFormat.WEBP;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_paths"><see cref="Bitmap"/> path</param>
        /// <param name="_format"><see cref="Bitmap"/> format</param>
        /// <param name="_quality"><see cref="Bitmap"/> compression quality</param>
        /// <param name="_alpha"><see cref="Bitmap"/> alpha boolean</param>
        /// <param name="_compression"><see cref="Bitmap"/> compression boolean</param>
        public BitmapConfig(string[] _paths, TextureFormat _format = TextureFormat.JPG, EncoderValue _compressionType = EncoderValue.CompressionLZW,
            int _quality = 255, bool _alpha = false, bool _compression = false)
        {
            TexturePaths = _paths;
            Format = _format;
            CompressionType = _compressionType;
            CompressQuality = _quality;
            UseAlpha = _alpha;
            UseCompression = _compression;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get <see cref="EncoderParameters"/> vlid with the current <see cref="TextureFormat"/>
        /// </summary>
        /// <returns>A list of <see cref="EncoderParameter"/> valid with the current <see cref="TextureFormat"/></returns>
        public EncoderParameters GetEncoderParameters()
        {
            if (!UseCompression)
                return null;

            EncoderParameters _params = new EncoderParameters(1);
            if (IsLossyFormat)
                _params.Param[0] = new EncoderParameter(Encoder.Quality, CompressQuality);
            else
                _params.Param[0] = new EncoderParameter(Encoder.Compression, (long)CompressionType);

            return _params;
        }

        /// <summary>
        /// Cast a <see cref="string"/> in <see cref="TextureFormat"/>
        /// </summary>
        /// <param name="_str"><see cref="string"/> to cast</param>
        /// <returns><see cref="TextureFormat"/> from the casted <see cref="string"/> (By default returns <see cref="TextureFormat"/>.JPG)</returns>
        public TextureFormat StringToFormat(string _str)
        {
            switch (_str)
            {
                case ".jpg":
                default:
                    return TextureFormat.JPG;
                case ".png":
                    return TextureFormat.PNG;
                case ".tga":
                    return TextureFormat.TGA;
                case ".tiff":
                    return TextureFormat.TIFF;
                case ".webp":
                    return TextureFormat.WEBP;
            }
        }

        /// <summary>
        /// Cast a <see cref="TextureFormat"/> in <see cref="string"/>
        /// </summary>
        /// <param name="_format"><see cref="TextureFormat"/> to cast</param>
        /// <returns><see cref="string"/> value of the casted <see cref="TextureFormat"/>
        /// (By default, returns .png</returns>
        public string FormatToString(TextureFormat _format)
        {
            switch(_format)
            {
                case TextureFormat.JPG:
                    return ".jpg";
                case TextureFormat.PNG:
                default:
                    return ".png";
                case TextureFormat.TGA:
                    return ".tga";
                case TextureFormat.TIFF:
                    return ".tiff";
                case TextureFormat.WEBP:
                    return ".webp";
            }
        }

        /// <summary>
        /// Get an <see cref="EncoderValue"/> for compression from a <see cref="string"/>
        /// </summary>
        /// <param name="_str"><see cref="string"/> to cast</param>
        /// <returns>Returns an <see cref="EncoderValue"/> containing a compression type (By default LZW)</returns>
        public EncoderValue CompressionTypeFromString(string _str)
        {
            switch (_str)
            {
                case "LZW":
                default:
                    return EncoderValue.CompressionLZW;
                case "RLE":
                    return EncoderValue.CompressionRle;
                case "CCITT3":
                    return EncoderValue.CompressionCCITT3;
                case "CCITT4":
                    return EncoderValue.CompressionCCITT4;
            }
        }
        #endregion
    }
}
