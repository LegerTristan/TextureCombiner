using NUnit.Framework;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TextureCombiner.Tests
{
    [TestFixture]
    public class BitmapIOTests
    {
        #region UnitTests
        [Test]
        public void SaveBitmap_Bitmap_Valid_Exists()
        {
            // Arrange
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            string _folderPath = _bitmapIO.DefaultFolder;
            string _completePath = Path.Combine(_folderPath, _bitmapIO.DefaultFileName + ".test");

            // Act
            _bitmapIO.SaveBitmap(_validBitmap, _completePath, _folderPath);

            // Assert
            Assert.IsTrue(File.Exists(_completePath), "Bitmap should be created.");
            File.Delete(_completePath); // Always delete file after test in order to not interfere with the next tests.
        }

        [Test]
        public void SaveBitmap_Bitmap_Null_ThrowsException()
        {
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _invalidBitmap = null;
            string _completePath = _bitmapIO.DefaultFileName;
            string _folderPath = _bitmapIO.DefaultFolder;

            Assert.Throws<TextureCombinerException>(() =>
                _bitmapIO.SaveBitmap(_invalidBitmap, _completePath, _folderPath));
        }

        [Test]
        public void SaveBitmap_File_Invalid_ThrowsException()
        {
            // Arrange
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            string _folderPath = _bitmapIO.DefaultFolder;
            string _completePath = "*/?";

            // Assert
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bitmapIO.SaveBitmap(_validBitmap, _completePath, _folderPath));
        }

        [Test]
        public void SaveBitmap_File_Null_ThrowsException()
        {
            // Arrange
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            string _folderPath = _bitmapIO.DefaultFolder;

            // Assert
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _bitmapIO.SaveBitmap(_validBitmap, null, _folderPath));
        }

        [Test]
        public void SaveBitmap_FolderPath_Valid_Exists()
        {
            // Arrange
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            string _folderPath = _bitmapIO.DefaultFolder;
            string _completePath = Path.Combine(_folderPath, _bitmapIO.DefaultFileName + ".test");

            // Act
            _bitmapIO.SaveBitmap(_validBitmap, _completePath, _folderPath);

            // Assert
            Assert.IsTrue(Directory.Exists(_folderPath), "Directory should be created.");
            File.Delete(_completePath);
        }

        [Test]
        public void SaveBitmap_FolderPath_Null_ThrowsException()
        {
            // Arrange
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _invalidBitmap = null;
            string _folderPath = null;
            string _completePath = "ValidPath.test";

            // Assert
            // Act & Assert
            Assert.Throws<TextureCombinerException>(() =>
                _bitmapIO.SaveBitmap(_invalidBitmap, _completePath, _folderPath));
        }

        [Test]
        public void SaveBitmap_FolderPath_Invalid_ThrowsException()
        {
            // Arrange
            BitmapIO _bitmapIO = new BitmapIO(null);
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            string _folderPath = "*/?";
            string _completePath = Path.Combine(_folderPath, _bitmapIO.DefaultFileName + ".test");

            // Assert
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _bitmapIO.SaveBitmap(_validBitmap, _completePath, _folderPath));
        }
        #endregion
    }
}
