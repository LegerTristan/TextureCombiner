using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Tests
{
    [TestFixture]
    public class BitmapSourceExtensionTests
    {
        [Test]
        public void GetPixels_BitmapSource_Valid_ReturnsPixels()
        {
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            byte[] _pixels = _validBitmap.GetPixels();

            Assert.IsNotNull(_pixels, "Pixels array should be not null.");
            Assert.AreEqual(Utils.GetBitmapStride(_validBitmap) * _validBitmap.PixelHeight, _pixels.Length, "Pixels array should have the same length " +
                "that the product of the width and the height.");
        }

        [Test]
        public void ToImageSharp_BitmapSource_Valid_ReturnsImageSharp()
        {
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            Image _convertedBitmap = _validBitmap.ToImageSharp<Bgra32>();
            Image _defaultImage = new Image<Bgra32>(800, 800);

            Assert.IsNotNull(_convertedBitmap, "Converted bitmap should be not null.");
            Assert.AreEqual(_convertedBitmap.Width, _defaultImage.Width, "Width should be the same.");
            Assert.AreEqual(_convertedBitmap.Height, _defaultImage.Height, "Height should be the same.");
            Assert.AreEqual(_convertedBitmap.PixelType.BitsPerPixel, _defaultImage.PixelType.BitsPerPixel, "BitsPerPixel should be the same.");
            Assert.AreEqual(_convertedBitmap.PixelType.AlphaRepresentation, _defaultImage.PixelType.AlphaRepresentation, "AlphaRepresentation should be the same.");
        }

        /// <summary>
        /// Test if the Resize method returns a valid bitmap with a valid and different size (width and height) 
        /// from the original bitmap.
        /// </summary>
        [Test]
        public void Resize_Parameters_Valid_ReturnsResizedBitmap()
        {
            int _expectedWidth = 1024, _expectedHeight = 1024;
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            BitmapSource _resizedBitmap = _validBitmap.Resize(_expectedWidth, _expectedHeight);

            Assert.IsNotNull(_resizedBitmap, "Resized bitmap should be not null.");
            Assert.AreEqual(_resizedBitmap.PixelWidth, _expectedWidth, "Width should be the same.");
            Assert.AreEqual(_resizedBitmap.PixelHeight, _expectedHeight, "Height should be the same.");
        }

        /// <summary>
        /// Test if the Resize method returns an invalid bitmap with an invalid width (width < 0) 
        /// from the original bitmap.
        /// </summary>
        [Test]
        public void Resize_Width_Invalid_ThrowsException()
        {
            int _expectedWidth = -50, _expectedHeight = 1024;
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);

            Assert.Throws<TextureCombinerException>(() => _validBitmap.Resize(_expectedWidth, _expectedHeight));
        }

        /// <summary>
        /// Test if the Resize method returns an invalid bitmap with an invalid height (height < 0) 
        /// from the original bitmap.
        /// </summary>
        [Test]
        public void Resize_Height_Invalid_ThrowsException()
        {
            int _expectedWidth = 1024, _expectedHeight = -2630;
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);

            Assert.Throws<TextureCombinerException>(() => _validBitmap.Resize(_expectedWidth, _expectedHeight));
        }

        [Test]
        public void Resize_WidthAndHeight_SameSize_ReturnsSameBitmap()
        {
            int _expectedWidth = 800, _expectedHeight = 800;
            BitmapSource _validBitmap = new WriteableBitmap(_expectedWidth, _expectedHeight, 72, 72, PixelFormats.Bgra32, null);
            BitmapSource _resizedBitmap = _validBitmap.Resize(_expectedWidth, _expectedHeight);

            Assert.AreEqual(_validBitmap, _resizedBitmap, "Bitmaps should be the same.");
        }
    }
}
