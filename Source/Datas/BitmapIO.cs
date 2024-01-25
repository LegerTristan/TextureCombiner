﻿using System;
using System.IO;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;

namespace TextureCombiner
{
    /// <summary>
    /// Read/Write in the environment a <see cref="Bitmap"/> file.
    /// </summary>
    public class BitmapIO
    {
        #region Properties
        public string DefaultFileName => "Default";
        public string DefaultFolder => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        #endregion

        #region Methods
        void CreateEnvironment(string _folder)
        {
            if (string.IsNullOrWhiteSpace(_folder))
                throw new TextureCombinerException("Folder path is invalid ! Check your save path and your file name.");

            Directory.CreateDirectory(_folder);
        }

        public void SaveBitmap(BitmapSource _bitmap, BitmapConfig _config, string _completePath, string _folderPath)
        {
            if(_bitmap == null)
                throw new TextureCombinerException("There is no generated bitmap! You need to make one before save it.");

            CreateEnvironment(_folderPath);

            IImageEncoder _encoder = GetEncoder(_config.TextureFormat);
            if (_encoder == null)
                throw new TextureCombinerException("There is no convenient encoder for this type of image !");

            WriteBitmap(_config.ConfigureImage(_bitmap), _encoder, _completePath);
        }

        /// <summary>
        /// Write an image at the path in parameters
        /// </summary>
        /// <param name="_completePath"><see cref="Bitmap"/> path</param>
        /// <param name="_bitmap"><see cref="Bitmap"/> to save</param>
        void WriteBitmap(Image _imgSharp, IImageEncoder _encoder, string _completePath)
        {
            if (_imgSharp != null && _encoder != null)
                _imgSharp.Save(_completePath, _encoder);
        }

        IImageEncoder GetEncoder(TextureFormat _format)
        {
            IImageEncoder _encoder = null;

            switch(_format)
            {
                case TextureFormat.PNG:
                    _encoder = new PngEncoder();
                    break;
                case TextureFormat.TIFF:
                    _encoder = new TiffEncoder();
                    break;
                case TextureFormat.TGA:
                    _encoder = new TgaEncoder();
                    break;
                case TextureFormat.BMP:
                    _encoder = new BmpEncoder();
                    break;
                case TextureFormat.JPG:
                    _encoder = new JpegEncoder();
                    break;
                default:
                    break;
            }

            return _encoder;
        }
        #endregion
    }
}
