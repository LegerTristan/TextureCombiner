using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner
{
    struct PixelComponents
    {
        public byte First { get; }
        public byte Second { get; }
        public byte Third { get; }
        public byte Fourth { get; }

        public PixelComponents(byte _first, byte _second, byte _third, byte _fourth)
        {
            First = _first;
            Second = _second;
            Third = _third;
            Fourth = _fourth;
        }
    }

    public class BitmapGenerator
    {
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
            FillBitmap(ref _bitmap, _srcs, _config.AuthorizedPixelFormat, _config.GetNbrCanals());

            return _bitmap.Resize(_config.Width, _config.Height);
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

        void FillBitmap(ref WriteableBitmap _bitmap, BitmapSource[] _bitmaps, AuthorizedPixelFormat _format,
            int _nbrCanals)
        {
            int _intendedNbrCoresToUse = Environment.ProcessorCount - 2;
            _intendedNbrCoresToUse = _intendedNbrCoresToUse <= 0 ? 1 : _intendedNbrCoresToUse;

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

                    int _partHeight = _bitmapHeight / _intendedNbrCoresToUse;

                    Parallel.For(0, _intendedNbrCoresToUse, _partIndex =>
                    {
                        int _startY = _partIndex * _partHeight;
                        int _endY = (_partIndex + 1) * _partHeight;

                        for (int i = 0; i < _bitmapWidth; i++)
                        {
                            for (int j = _startY; j < _endY; j++)
                            {
                                int _pixelIndex = (j * _stride) + (i * _nbrCanals);

                                byte _blue = _pixelsArray[2][_pixelIndex];
                                byte _green = _pixelsArray[1][_pixelIndex + 1];
                                byte _red = _pixelsArray[0][_pixelIndex + 2];
                                byte _alpha = 0;

                                if (_useAlpha)
                                    _alpha = _pixelsArray[3][_pixelIndex + 3];

                                int _canalIndex = ((j * _bitmapWidth) + i) * _nbrCanals;

                                PixelComponents _components = ArrangeComponents(_red, _green, _blue, _alpha, _format);

                                _ptrDest[_canalIndex] = _components.First;
                                _ptrDest[_canalIndex + 1] = _components.Second;
                                _ptrDest[_canalIndex + 2] = _components.Third;

                                if (_useAlpha)
                                    _ptrDest[_canalIndex + 3] = _components.Fourth;
                            }
                        }
                    });

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

        PixelComponents ArrangeComponents(byte _red, byte _green, byte _blue, byte _alpha, 
            AuthorizedPixelFormat _format)
        {
            PixelComponents _pixelComps;

            switch(_format)
            {
                case AuthorizedPixelFormat.BGR24:
                case AuthorizedPixelFormat.BGRA32:
                case AuthorizedPixelFormat.RGBA32:
                default:
                    _pixelComps = new PixelComponents(_blue, _green, _red, _alpha);
                    break;
                case AuthorizedPixelFormat.RGB24:
                case AuthorizedPixelFormat.RGB48:
                    _pixelComps = new PixelComponents(_red, _green, _blue, _alpha);
                    break;
            }

            return _pixelComps;
        }
        #endregion
    }
}
