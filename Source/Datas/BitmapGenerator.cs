using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner
{
    //class BitmapColorizationHandler
    //{
    //    Bitmap[] bitmapsToInject = null;

    //    Bitmap bitmapToColorize = null;

    //    int bitmapWidth = 0;

    //    int bitmapHeight = 0;

    //    public BitmapColorizationHandler(Bitmap[] _bitmaps, int _width, int _height)
    //    {
    //        bitmapsToInject = _bitmaps;
    //        bitmapWidth = _width;
    //        bitmapHeight = _height;
    //    }

    //    public Bitmap InjectTextures(ref Bitmap _bitmap)
    //    {
    //        bitmapToColorize = _bitmap;

    //        Rectangle _rect = new Rectangle(0, 0, bitmapWidth, bitmapHeight);
    //        BitmapData _bitmapDataA = bitmapsToInject[0].LockBits(_rect, ImageLockMode.ReadOnly, bitmapsToInject[0].PixelFormat),
    //                   _bitmapDataB = bitmapsToInject[1].LockBits(_rect, ImageLockMode.ReadOnly, bitmapsToInject[1].PixelFormat),
    //                   _bitmapDataC = bitmapsToInject[2].LockBits(_rect, ImageLockMode.ReadOnly, bitmapsToInject[2].PixelFormat),
    //                   _bitmapDataDest = bitmapToColorize.LockBits(_rect, ImageLockMode.ReadWrite, bitmapToColorize.PixelFormat);

    //        try
    //        {
    //            unsafe
    //            {
    //                byte* _ptrA = (byte*)_bitmapDataA.Scan0,
    //                      _ptrB = (byte*)_bitmapDataB.Scan0,
    //                      _ptrC = (byte*)_bitmapDataC.Scan0,
    //                      _ptrDest = (byte*)_bitmapDataDest.Scan0;

    //                int _bytesPerPixel = Image.GetPixelFormatSize(_bitmap.PixelFormat) / 8;

    //                for (int i = 0; i < bitmapWidth; i++)
    //                {
    //                    int _line = i * _bytesPerPixel;

    //                    for (int j = 0; j < bitmapHeight; j++)
    //                    {
    //                        int _offsetA = (j * _bitmapDataA.Stride) + _line,
    //                            _offsetB = (j * _bitmapDataB.Stride) + _line,
    //                            _offsetC = (j * _bitmapDataC.Stride) + _line,
    //                            _offsetDest = (j * _bitmapDataDest.Stride) + _line;

    //                        byte _blue = _ptrC[_offsetC];
    //                        byte _green = _ptrB[_offsetB];
    //                        byte _red = _ptrA[_offsetA];

    //                        _ptrDest[_offsetDest] = _blue;
    //                        _ptrDest[_offsetDest + 1] = _green;
    //                        _ptrDest[_offsetDest + 2] = _red;
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception _ex)
    //        {
    //            Console.WriteLine(_ex);
    //        }
    //        finally
    //        {
    //            bitmapsToInject[0].UnlockBits(_bitmapDataA);
    //            bitmapsToInject[1].UnlockBits(_bitmapDataB);
    //            bitmapsToInject[2].UnlockBits(_bitmapDataC);
    //            bitmapToColorize.UnlockBits(_bitmapDataDest);
    //        }

    //        return bitmapToColorize;
    //    }
    //}

    public class BitmapGenerator
    {
        #region DefaultValues
        public string DefaultFileName => "Default";
        public string DefaultFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TextureCombiner");
        #endregion

        #region F/P

        #region Delegates
        /// <summary>
        /// <see cref="Action"/> invoked when progress change during Generate method.
        /// </summary>
        public event Action OnGenerationStart = null;

        /// <summary>
        /// <see cref="Action"/> invoked when Generated method was complete.
        /// </summary>
        public event Action<BitmapSource> OnGenerationCompleted = null;
        #endregion

        #region Constants
        const int NUMBER_CHANNELS_RGB = 3;
        const int NUMBER_CHANNELS_ARGB = 4;

        const int GREEN_CANAL_OFFSET = 8;
        const int RED_CANAL_OFFSET = 16;
        const int ALPHA_CANAL_OFFSET = 24;
        #endregion

        /// <summary>
        /// <see cref="Bitmap"/> generated after adding at least 3 textures in the program
        /// </summary>
        BitmapSource generatedBitmap = null;

        public BitmapSource GeneratedBitmap => generatedBitmap;
        #endregion

        #region Methods

        /// <summary>
        /// Generate a <see cref="Bitmap"/> based on a <see cref="BitmapConfig"/>.
        /// </summary>
        /// <param name="_config">Config for the <see cref="Bitmap"/> to generate</param>
        public void GenerateBitmap(BitmapConfig _config)
        {
            OnGenerationStart?.Invoke();
            generatedBitmap = CreateBitmap(_config);
            OnGenerationCompleted?.Invoke(generatedBitmap);
        }

        BitmapSource CreateBitmap(BitmapConfig _config)
        {
            if (!IsBitmapsValid(_config.Textures, _config.GetNbrCanals()))
                throw new TextureCombinerException("RGB is invalid ! Be sure that every texture has the same resolution.");

            PixelFormat _format = _config.GetPixelFormat();

            BitmapSource[] _srcs = new BitmapSource[4]
            {
                new FormatConvertedBitmap(_config.Textures[0], _format, null, 0),
                new FormatConvertedBitmap(_config.Textures[1], _format, null, 0),
                new FormatConvertedBitmap(_config.Textures[2], _format, null, 0),
                _config.Textures[3] == null ? null : new FormatConvertedBitmap(_config.Textures[3], _format, null, 0),
            };

            WriteableBitmap _bitmap = new WriteableBitmap(_srcs[0].PixelWidth, _srcs[0].PixelHeight,
                _srcs[0].DpiX, _srcs[0].DpiY, _format, null);
            FillBitmap(ref _bitmap, _srcs, _config.GetNbrCanals());

            return _bitmap;
        }

        /// <summary>
        /// Check that every <see cref="Bitmap"/> have the same size.
        /// </summary>
        /// <param name="_bitmaps"><see cref="Bitmap"/> array</param>
        /// <param name="_length">Number of textures to check, for RGB and ARGB</param>
        /// <returns></returns>
        bool IsBitmapsValid(BitmapSource[] _bitmaps, int _length = NUMBER_CHANNELS_RGB)
        {
            int _width = _bitmaps[0].PixelWidth, _height = _bitmaps[0].PixelHeight;

            for (int i = 0; i < _length; ++i)
            {
                if (_bitmaps[i].PixelWidth != _width || _bitmaps[i].PixelHeight != _height)
                    return false;
            }

            return true;
        }

        void FillBitmap(ref WriteableBitmap _bitmap, BitmapSource[] _bitmaps, int _nbrCanals)
        {
            try
            {
                unsafe
                {
                    _bitmap.Lock();

                    int _bitmapWidth = _bitmap.PixelWidth,
                        _bitmapHeight = _bitmap.PixelHeight,
                        _stride = _bitmap.BackBufferStride;
                    bool _useAlpha = _nbrCanals == 4;

                    byte* _ptrDest = (byte*)_bitmap.BackBuffer;
                    byte[][] _pixelsArray = PreparePixelsArrays(_bitmaps, _bitmapHeight, _stride);

                    for (int i = 0; i < _bitmapWidth; i++)
                    {
                        for (int j = 0; j < _bitmapHeight; j++)
                        {
                            int _pixelIndex = (j * _stride) + (i * _nbrCanals);

                            byte _blue = _pixelsArray[2][_pixelIndex];
                            byte _green = _pixelsArray[1][_pixelIndex + 1];
                            byte _red = _pixelsArray[0][_pixelIndex + 2];
                           
                            int _canalIndex = ((j * _bitmapWidth) + i) * _nbrCanals;

                            _ptrDest[_canalIndex] = _blue;
                            _ptrDest[_canalIndex + 1] = _green;
                            _ptrDest[_canalIndex + 2] = _red;

                            if (_useAlpha)
                            {
                                byte _alpha = _pixelsArray[3][_pixelIndex + 3];
                                _ptrDest[_canalIndex + 3] = _alpha;
                            }
                        }
                    }
                }
            }
            finally
            {
                _bitmap.Unlock();
            }
        }

        byte[][] PreparePixelsArrays(BitmapSource[] _bitmaps, int _bitmapHeight, int _stride)
        {
            int _size = _bitmapHeight * _stride;

            byte[][] _pixelsArray = new byte[][]
            {
                        new byte[_size],
                        new byte[_size],
                        new byte[_size],
                        new byte[_size]
            };

            for(int i = 0; i < _bitmaps.Length; ++i)
                if(_bitmaps[i] != null)
                    _bitmaps[i].CopyPixels(_pixelsArray[i], _stride, 0);
  

            return _pixelsArray;
        }
 
        #region UnitTest
        //public void GenerateBitmap_UT(string _bitmapName, string _folderPath)
        //{
        //    //Bitmap _bitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);
        //    //ColorBitmap_UT(ref _bitmap);
        //    //BitmapIO _io = new BitmapIO(_folderPath);
        //    //_io.WriteBitmap(Path.Combine(_folderPath, _bitmapName) + DefaultEXT, _bitmap, new EncoderParameters());
        //}

        //void ColorBitmap_UT(ref Bitmap _bitmap)
        //{
        //    for (int i = 0; i < _bitmap.Width; i++)
        //    {
        //        for (int j = 0; j < _bitmap.Height; j++)
        //        {
        //            _bitmap.SetPixel(i, j, Color.White);
        //        }
        //    }
        //}
        #endregion

        #endregion
    }
}
