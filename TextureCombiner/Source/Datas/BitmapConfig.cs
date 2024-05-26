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
        PNG     = 1,
        TGA     = 2,
        TIFF    = 4,
        BMP     = 8,
        JPG     = 16
    }

    public enum AuthorizedPixelFormat
    {
        BGRA32  = 1,
        BGR24   = 2,
        RGB24   = 4,
        RGB48   = 8,
        RGBA32  = 16,
    }

    /// <summary>
    /// Parameters to config a bitmap such as format, size, and use base texture's path.
    /// </summary>
    public class BitmapConfig : SingletonTemplate<BitmapConfig>
    {
        #region F/P
        public event Action<TextureFormat> OnTextureFormatChanged = null;
        public event Action<AuthorizedPixelFormat> OnPixelFormatChanged = null;
        public event Action OnSizeExceed = null;

        public const int MAX_SIZE = 8192;

        BitmapSource[] textures = new BitmapSource[4];

        public BitmapSource[] Textures => textures;

        TextureFormat textureFormat = TextureFormat.TGA;

        AuthorizedPixelFormat pixelFormat = AuthorizedPixelFormat.BGR24;

        AuthorizedPixelFormat currentBitmapPixelFormat = AuthorizedPixelFormat.BGR24;

        public TextureFormat TextureFormat => textureFormat;

        public AuthorizedPixelFormat AuthorizedPixelFormat => pixelFormat;

        int width = 1024;

        int height = 1024;

        public int Width => width;

        public int Height => height;

        #endregion

        #region Methods

        public void SetTextureAt(BitmapSource _src, int _index)
        {
            if (_index < 0 || _index >= textures.Length)
                return;

            textures[_index] = _src;
            if (DoesSizeExceedCurrentTextures())
                OnSizeExceed?.Invoke();
        }

        public void SetWidth(int _newValue)
        {
            width = _newValue;
            if (DoesSizeExceedCurrentTextures())
                OnSizeExceed?.Invoke();
        }

        public void SetHeight(int _newValue)
        {
            height = _newValue;
            if (DoesSizeExceedCurrentTextures())
                OnSizeExceed?.Invoke();
        }

        bool DoesSizeExceedCurrentTextures()
        {
            foreach(BitmapSource _src in Textures)
            {
                if (_src != null && (_src.PixelWidth < width || _src.PixelHeight < height))
                    return true;
            }

            return false;
        }

        public void UpdateBitmapPixelFormat() => currentBitmapPixelFormat = pixelFormat;

        public PixelFormat GetPixelFormat()
        {
            switch (pixelFormat)
            {
                case AuthorizedPixelFormat.BGR24:
                default:
                    return PixelFormats.Bgr24;
                case AuthorizedPixelFormat.BGRA32:
                case AuthorizedPixelFormat.RGBA32:
                    return PixelFormats.Bgra32;
                case AuthorizedPixelFormat.RGB24:
                    return PixelFormats.Rgb24;
                case AuthorizedPixelFormat.RGB48:
                    return PixelFormats.Rgb48;
            }
        }

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
                case ".bmp":
                    return TextureFormat.BMP;
                case ".jpg":
                    return TextureFormat.JPG;
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
                case TextureFormat.BMP:
                    return ".bmp";
                case TextureFormat.JPG:
                    return ".jpeg";
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
                case "RGBA32":
                    return AuthorizedPixelFormat.RGBA32;
                case "RGB24":
                    return AuthorizedPixelFormat.RGB24;
                case "RGB48":
                    return AuthorizedPixelFormat.RGB48;
                case "BGR24":
                default:
                    return AuthorizedPixelFormat.BGR24;
            }
        }

        public Image ConfigureImage(BitmapSource _src)
        {
            Image _img = null;
            switch(currentBitmapPixelFormat)
            {
                case AuthorizedPixelFormat.BGRA32:
                    _img = _src.ToImageSharp<Bgra32>();
                    break;
                case AuthorizedPixelFormat.RGBA32:
                    Image _tempImg = _src.ToImageSharp<Bgra32>();
                    _img = _tempImg.CloneAs<Rgba32>();
                    break;
                case AuthorizedPixelFormat.RGB24:
                    _img = _src.ToImageSharp<Rgb24>();
                    break;
                case AuthorizedPixelFormat.RGB48:
                    _img = _src.ToImageSharp<Rgb48>();
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
                case AuthorizedPixelFormat.RGBA32:
                    return 4;
                case AuthorizedPixelFormat.BGR24:
                default:
                    return 3;
            }
        }
        #endregion
    }
}
