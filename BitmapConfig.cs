using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner
{
    /// <summary>
    /// Formats that can handle the program
    /// </summary>
    public enum TextureFormat
    {
        PNG,
        TGA,
        TIFF
    }

    public enum AuthorizedPixelFormat
    {
        BGRA32,
        BGR24
    }

    /// <summary>
    /// Parameters to config a bitmap such as format, size, and use base texture's path.
    /// </summary>
    public class BitmapConfig
    {
        #region Constants
        public const int MIN_SIZE = 1, MAX_SIZE = 4096;
        #endregion

        #region F/P
        public event Action<TextureFormat> OnTextureFormatChanged = null;
        public event Action<AuthorizedPixelFormat> OnPixelFormatChanged = null;

        public BitmapSource[] Textures { get; set; }

        TextureFormat textureFormat = TextureFormat.TGA;

        public TextureFormat TextureFormat => textureFormat;

        AuthorizedPixelFormat pixelFormat = AuthorizedPixelFormat.BGR24;

        AuthorizedPixelFormat currentBitmapPixelFormat = AuthorizedPixelFormat.BGR24;

        public PixelFormat GetPixelFormat()
        {
            switch (pixelFormat)
            {
                case AuthorizedPixelFormat.BGR24:
                default:
                    return PixelFormats.Bgr24;
                case AuthorizedPixelFormat.BGRA32:
                    return PixelFormats.Bgra32;
            }
        }

        public void SetBitmapPixelFormat(string _strFormat) => currentBitmapPixelFormat = 
            StringToPixelFormat(_strFormat);

        public int CompressQuality { get; set; }
        public bool UseCompression { get; set; }

        #endregion

        #region Constructor
        public BitmapConfig(BitmapImage[] _textures, string _strFormat, string _strPixelFormat, 
            int _quality = 255, bool _compression = false)
        {
            Textures = _textures;
            textureFormat = StringToTextureFormat(_strFormat);
            pixelFormat = StringToPixelFormat(_strPixelFormat);
            CompressQuality = _quality;
            UseCompression = _compression;
        }
        #endregion

        #region Methods

        public void SetTextureFormat(string _strFormat)
        {
            textureFormat = StringToTextureFormat(_strFormat);
            OnTextureFormatChanged?.Invoke(textureFormat);
        }

        /// <summary>
        /// Cast a <see cref="string"/> in <see cref="TextureFormat"/>
        /// </summary>
        /// <param name="_str"><see cref="string"/> to cast</param>
        /// <returns><see cref="TextureFormat"/> from the casted <see cref="string"/> (By default returns <see cref="TextureFormat"/>.JPG)</returns>
        TextureFormat StringToTextureFormat(string _str)
        {
            switch (_str)
            {
                case ".png":
                    return TextureFormat.PNG;
                case ".tga":
                default:
                    return TextureFormat.TGA;
                case ".tiff":
                    return TextureFormat.TIFF;
            }
        }

        /// <summary>
        /// Cast a <see cref="TextureFormat"/> in <see cref="string"/>
        /// </summary>
        /// <returns><see cref="string"/> value of the casted <see cref="TextureFormat"/>
        /// (By default, returns .png</returns>
        public string TextureFormatToString()
        {
            switch(textureFormat)
            {
                case TextureFormat.PNG:
                default:
                    return ".png";
                case TextureFormat.TGA:
                    return ".tga";
                case TextureFormat.TIFF:
                    return ".tiff";
            }
        }

        public void SetPixelFormat(string _strFormat)
        {
            pixelFormat = StringToPixelFormat(_strFormat);
            OnPixelFormatChanged?.Invoke(pixelFormat);
        }

        AuthorizedPixelFormat StringToPixelFormat(string _str)
        {
            switch (_str)
            {
                case "BGRA32":
                    return AuthorizedPixelFormat.BGRA32;
                case "BGR24":
                default:
                    return AuthorizedPixelFormat.BGR24;
            }
        }

        public bool UseAlpha()
        {
            return AuthorizedPixelFormat.BGRA32 == pixelFormat;
        }

        public Image ConfigureImage(BitmapSource _src)
        {
            Image _img = null;
            switch(currentBitmapPixelFormat)
            {
                case AuthorizedPixelFormat.BGRA32:
                    _img = _src.ToImageSharp<Bgra32>();
                    break;
                case AuthorizedPixelFormat.BGR24:
                default:
                    _img = _src.ToImageSharp<Bgr24>();
                    break;
            }

            return _img;
        }

        public int GetNbrCanals()
        {
            switch (pixelFormat)
            {
                case AuthorizedPixelFormat.BGRA32:
                    return 4;
                case AuthorizedPixelFormat.BGR24:
                default:
                    return 3;
            }
        }
        #endregion
    }
}
