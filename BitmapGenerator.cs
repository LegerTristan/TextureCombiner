using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using TGASharpLib;

namespace TextureCombiner
{
    public class BitmapGenerator
    {
        #region Delegates
        /// <summary>
        /// <see cref="Action"/> invoked when progress change during Generate method.
        /// </summary>
        public event Action<float> OnGenerationProgress = null;

        /// <summary>
        /// <see cref="Action"/> invoked when Generated method was complete.
        /// </summary>
        public event Action OnGenerationCompleted = null;
        #endregion

        #region Constants
        const int NUMBER_CHANNELS_RGB = 3;
        const int NUMBER_CHANNELS_RGBA = 4;
        #endregion

        #region DefaultValues
        public string DefaultFileName => "Default";
        public string DefaultFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TextureCombiner");
        #endregion

        #region F/P
        /// <summary>
        /// <see cref="Bitmap"/> generated after adding at least 3 textures in the program
        /// </summary>
        public Bitmap GeneratedBitmap { get; set; } = null;
        #endregion

        #region Methods
        Bitmap[] GetBitmapsFromPaths(string[] _texturePaths, int _maxLength = 3)
        {
            Bitmap[] _bitmaps = new Bitmap[_maxLength];
            for (int i = 0; i < _texturePaths.Length && i < _maxLength; ++i)
            {
                if(string.IsNullOrWhiteSpace(_texturePaths[i]))
                    throw new TextureCombinerException($"Texture's path at index {i} is invalid !" +
                        " Change the texture in order to fix it.");

                _bitmaps[i] = new Bitmap(_texturePaths[i]);
            }

            return _bitmaps;
        }

        /// <summary>
        /// Generate a <see cref="Bitmap"/> based on a <see cref="BitmapConfig"/>.
        /// </summary>
        /// <param name="_config">Config for the <see cref="Bitmap"/> to generate</param>
        public void GenerateBitmap(BitmapConfig _config)
        {
            if (_config.UseAlpha)
                GeneratedBitmap = CreateBitmap32bpp(_config);
            else
                GeneratedBitmap = CreateBitmap24bpp(_config);

            OnGenerationCompleted?.Invoke();
        }

        /// <summary>
        /// Save a <see cref="Bitmap"/> file at the path in params.
        /// </summary>
        /// <param name="_completePath">Complete path of the new <see cref="Bitmap"/></param>
        /// <param name="_folderPath">Environment of the <see cref="Bitmap"/></param>
        public void SaveBitmap(string _completePath, string _folderPath, BitmapConfig _config)
        {
            if (GeneratedBitmap == null)
                throw new TextureCombinerException("There is no generated bitmap ! You need to generate one before save it.");
            
            switch(_config.Format)
            {
                case TextureFormat.JPG:
                case TextureFormat.PNG:
                case TextureFormat.TIFF:
                default:
                    new BitmapIO(_folderPath).WriteBitmap(_completePath, GeneratedBitmap, _config.GetEncoderParameters());
                    break;
                case TextureFormat.TGA:
                    new BitmapIO(_folderPath).WriteTGA(_completePath, new TGA(GeneratedBitmap, _config.UseCompression));
                    break;
            }

            
        }

        /// <summary>
        /// Check that every <see cref="Bitmap"/> have the same size.
        /// </summary>
        /// <param name="_bitmaps"><see cref="Bitmap"/> array</param>
        /// <param name="_length">Number of textures to check, for RGB and ARGB</param>
        /// <returns></returns>
        bool IsBitmapsValid(Bitmap[] _bitmaps, int _length = NUMBER_CHANNELS_RGB)
        {
            int _width = _bitmaps[0].Width, _height = _bitmaps[0].Height;

            for (int i = 0; i < _length; ++i)
            {
                if (_bitmaps[i].Width != _width || _bitmaps[i].Height != _height)
                    return false;
            }

            return true;
        }

        #region 24bpp

        Bitmap CreateBitmap24bpp(BitmapConfig _config)
        {
            Bitmap[] _bitmaps = GetBitmapsFromPaths(_config.TexturePaths, NUMBER_CHANNELS_RGB);

            if (!IsBitmapsValid(_bitmaps, NUMBER_CHANNELS_RGB))
                throw new TextureCombinerException("RGB is invalid ! Be sure that every texture has the same resolution.");

            Bitmap _bitmap = new Bitmap(_bitmaps[0].Width, _bitmaps[0].Height, PixelFormat.Format24bppRgb);
            ColorBitmap24bpp(ref _bitmap, _bitmaps);

            return _bitmap;
        }

        void ColorBitmap24bpp(ref Bitmap _bitmap, Bitmap[] _bitmaps)
        {
            int _bitmapSize = _bitmap.Width * _bitmap.Height;

            for (int i = 0; i < _bitmap.Width; i++)  
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    var _pixels = new                           // For caching
                    {
                        zero = _bitmaps[0].GetPixel(i, j),
                        one = _bitmaps[1].GetPixel(i, j),
                        two = _bitmaps[2].GetPixel(i, j)
                    };

                    _bitmap.SetPixel(i, j, Color.FromArgb(
                                                _pixels.zero.R & _pixels.zero.G & _pixels.zero.B & _pixels.zero.A,
                                                _pixels.one.R & _pixels.one.G & _pixels.one.B & _pixels.one.A,
                                                _pixels.two.R & _pixels.two.G & _pixels.two.B & _pixels.two.A
                                            ));

                    OnGenerationProgress?.Invoke(((float)(i * _bitmap.Height) / _bitmapSize) * 100.0f);
                }
            }
        }

        #endregion

        #region 32bpp

        Bitmap CreateBitmap32bpp(BitmapConfig _config)
        {
            Bitmap[] _bitmaps = GetBitmapsFromPaths(_config.TexturePaths, NUMBER_CHANNELS_RGBA);
            if (!IsBitmapsValid(_bitmaps, NUMBER_CHANNELS_RGBA))
                throw new TextureCombinerException("ARGB is invalid ! Be sure that every texture has the same resolution.");

            Bitmap _bitmap = new Bitmap(_bitmaps[0].Width, _bitmaps[0].Height, PixelFormat.Format32bppArgb);
            ColorBitmap32bpp(ref _bitmap, _bitmaps);

            return _bitmap;
        }

        void ColorBitmap32bpp(ref Bitmap _bitmap, Bitmap[] _bitmaps)
        {
            int _bitmapSize = _bitmap.Width * _bitmap.Height;

            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    var _pixels = new                           // For caching
                    {
                        zero = _bitmaps[0].GetPixel(i, j),
                        one = _bitmaps[1].GetPixel(i, j),
                        two = _bitmaps[2].GetPixel(i, j),
                        three = _bitmaps[3].GetPixel(i, j)
                    };

                    _bitmap.SetPixel(i, j, Color.FromArgb(
                                                _pixels.three.A & _pixels.three.R & _pixels.three.G & _pixels.three.B,
                                                _pixels.zero.A & _pixels.zero.R & _pixels.zero.G & _pixels.zero.B,
                                                _pixels.one.A & _pixels.one.R & _pixels.one.G & _pixels.one.B,
                                                _pixels.two.A & _pixels.two.R & _pixels.two.G & _pixels.two.B
                                            ));

                    OnGenerationProgress?.Invoke(((float)(i * _bitmap.Height) / _bitmapSize) * 100.0f);
                }
            }
        }
        #endregion

        #region UnitTest
        public void GenerateBitmap_UT(string _bitmapName, string _folderPath)
        {
            Bitmap _bitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);
            ColorBitmap_UT(ref _bitmap);
            BitmapIO _io = new BitmapIO(_folderPath);
            //_io.WriteBitmap(Path.Combine(_folderPath, _bitmapName) + DefaultEXT, _bitmap, new EncoderParameters());
        }

        void ColorBitmap_UT(ref Bitmap _bitmap)
        {
            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    _bitmap.SetPixel(i, j, Color.White);
                }
            }
        }
        #endregion

        #endregion
    }
}
