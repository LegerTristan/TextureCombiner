using NUnit.Framework;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void LoadBitmapImage_Path_Valid_LoadBitmap()
        {
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            string _validPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Hello_World.bmp");
            BmpBitmapEncoder _encoder = new BmpBitmapEncoder();
            _encoder.Frames.Add(BitmapFrame.Create(_validBitmap));

            using (Stream _stream = File.Create(_validPath))
            {
                _encoder.Save(_stream);
            }

            BitmapImage _img = Utils.LoadBitmapImage(_validPath);
            Assert.IsNotNull(_img, "Loaded bitmap should be not null.");
            _img.StreamSource.Close();
            File.Delete(_validPath);
        }

        [Test]
        public void LoadBitmapImage_BitmapToLoad_Invalid_ThrowsException()
        {
            string _validPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Hello_World.bmp");
            File.Create(_validPath)?.Close();

            Assert.Throws<NotSupportedException>(() => Utils.LoadBitmapImage(_validPath));
            File.Delete(_validPath);
        }

        [Test]
        public void LoadBitmapImage_Path_Null_ThrowsException()
        {
            Assert.Throws<System.ArgumentNullException>(() => Utils.LoadBitmapImage(null));
        }

        [Test]
        public void LoadBitmapImage_Path_Invalid_ThrowsException()
        {
            try
            {
                BitmapImage _img = Utils.LoadBitmapImage("*/?");
                Assert.IsNull(_img.Format, "Loaded bitmap should be null.");
            }
            catch(Exception _exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void GetBitmapStride_BitmapSource_Valid_ReturnsCorrectValue()
        {
            WriteableBitmap _bitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            int _stride = Utils.GetBitmapStride(_bitmap);
            Assert.AreEqual(_bitmap.BackBufferStride, _stride, "Stride value should be equal with bitmap's stride.");
        }

        [Test]
        public void GetBitmapStride_BitmapSource_Invalid_ReturnsMinusOne()
        {
            int _stride = Utils.GetBitmapStride(null);

            Assert.AreEqual(-1, _stride, "Stride value should be -1.");
        }
    }
}
