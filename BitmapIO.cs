using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using TGASharpLib;
//using System;
//using System.Windows.Media.Effects;
//using System.Windows.Media.Imaging;
//using System.Windows.Interop;
//using System.Windows;
//using System.Text;

namespace TextureCombiner
{
    /// <summary>
    /// Read/Write in the environment a <see cref="Bitmap"/> file.
    /// </summary>
    public class BitmapIO
    {
        const string DEFAULT_ENCODER_EXTENSION = "*.png";

        #region Constructor
        public BitmapIO(string _folder)
        {
            CreateEnvironment(_folder);
        }
        #endregion

        #region Methods
        void CreateEnvironment(string _folder)
        {
            if (string.IsNullOrWhiteSpace(_folder))
                throw new TextureCombinerException("Folder path is invalid ! Check your save path and your file name.");

            Directory.CreateDirectory(_folder);
        }

        /// <summary>
        /// Write an image at the path in parameters
        /// </summary>
        /// <param name="_completePath"><see cref="Bitmap"/> path</param>
        /// <param name="_bitmap"><see cref="Bitmap"/> to save</param>
        public void WriteBitmap(string _completePath, Bitmap _bitmap, EncoderParameters _parameters)
        {
            string[] _parts = _completePath.Split('.');
            ImageCodecInfo _encoder = GetImageEncoder("*." + _parts[_parts.Length - 1].ToLower());

            if (_encoder != null)
                _bitmap.Save(_completePath, _encoder, _parameters);
            else
                _bitmap.Save(_completePath);


        }

        // Test with System.Windows.Media.Imaging library
        //public void WriteBitmap(string _completePath, Bitmap _bitmap, BitmapConfig _config)
        //{
        //    BitmapImage _image = Bitmap2BitmapImage(_bitmap, _config);

        //    BitmapEncoder _encoder = GetEncoder(_config);

        //    _encoder.Frames.Add(BitmapFrame.Create(_image));

        //    _encoder.Save(new FileStream(_completePath, FileMode.OpenOrCreate));
        //}

        //BitmapImage Bitmap2BitmapImage(Bitmap _bitmap, BitmapConfig _config)
        //{
        //    BitmapSource _src = Imaging.CreateBitmapSourceFromHBitmap(
        //                            _bitmap.GetHbitmap(),
        //                            IntPtr.Zero,
        //                            Int32Rect.Empty,
        //                            BitmapSizeOptions.FromEmptyOptions());

        //    BitmapEncoder _encoder = GetEncoder(_config);
        //    MemoryStream _memoryStream = new MemoryStream();
        //    BitmapImage _img = new BitmapImage();

        //    _encoder.Frames.Add(BitmapFrame.Create(_src));
        //    _encoder.Save(_memoryStream);

        //    _memoryStream.Position = 0;
        //    _img.BeginInit();
        //    _img.StreamSource = new MemoryStream(_memoryStream.ToArray());
        //    _img.EndInit();

        //    return _img;
        //}

        //BitmapEncoder GetEncoder(BitmapConfig _config)
        //{
        //    switch(_config.Format)
        //    {
        //        case TextureFormat.JPG:
        //        default:
        //            JpegBitmapEncoder _jpgEncoder = new JpegBitmapEncoder();
        //            _jpgEncoder.QualityLevel = _config.UseCompression ? _config.CompressQuality : 100;
        //            return _jpgEncoder;
        //        case TextureFormat.PNG:
        //            return new PngBitmapEncoder();
        //        case TextureFormat.TIFF:
        //            TiffBitmapEncoder _tiffEncoder = new TiffBitmapEncoder();
        //            _tiffEncoder.Compression = _config.UseCompression ? TiffCompressOption.Lzw : TiffCompressOption.None;
        //            return _tiffEncoder;
        //    }
        //}

        ImageCodecInfo GetImageEncoder(string _extension)
        {
            ImageCodecInfo[] _codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < _codecs.Length; i++)
            {
                if (_codecs[i].FilenameExtension.ToLower().Contains(_extension))
                        return _codecs[i];
            }

            return _codecs.FirstOrDefault((_codec) => _codec.FilenameExtension.Contains(DEFAULT_ENCODER_EXTENSION));
        }

        /// <summary>
        /// Write a <see cref="TGA"/> image at path in params using TGASharpLib
        /// </summary>
        /// <param name="_completePath">Save path</param>
        /// <param name="_tga"><see cref="TGA"/> to save></param>
        /// <exception cref="TextureCombinerException">TGA could not be created or saved.</exception>
        public void WriteTGA(string _completePath, TGA _tga)
        {
            if (_tga == null)
                throw new TextureCombinerException("TGA file could not be created !");

            if(!_tga.Save(_completePath))
                throw new TextureCombinerException("TGA file could not be saved !");
        }
        #endregion
    }
}
