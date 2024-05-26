using NUnit.Framework;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Tests
{
    [TestFixture]
    public class BitmapGeneratorTests
    {
        #region UnitTests
        [Test]
        public void GenerateBitmap_Config_Valid_ValidGeneratedBitmap()
        {
            //Arrange
            BitmapGenerator _generator = new BitmapGenerator();
            BitmapConfig _config = BitmapConfig.Instance;
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            int _size = _config.GetNbrCanals();
            for (int i = 0; i < _size; ++i)
                _config.SetTextureAt(_validBitmap, i);

            // Act
            _generator.GenerateBitmap();

            // Assert
            Assert.IsNotNull(_generator.GeneratedBitmap, "Generated bitmap should be not null.");
            Assert.AreEqual(_config.Width, _generator.GeneratedBitmap.PixelWidth, "Generated bitmap should have same width than indicated in config.");
            Assert.AreEqual(_config.Height, _generator.GeneratedBitmap.PixelHeight, "Generated bitmap should have same height than indicated in config.");
        }

        [Test]
        public void GenerateBitmap_Config_MissTextures_ThrowsException()
        {
            BitmapGenerator _generator = new BitmapGenerator();

            // Assert
            Assert.Throws<TextureCombinerException>(() => _generator.GenerateBitmap());
        }

        [Test]
        public void GenerateBitmap_Config_TexturesWithDifferentSize_ThrowsException()
        {
            BitmapGenerator _generator = new BitmapGenerator();
            BitmapConfig _config = BitmapConfig.Instance;
            BitmapSource _differentBitmap = new WriteableBitmap(1024, 1024, 72, 72, PixelFormats.Bgra32, null);
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            _config.SetTextureAt(_differentBitmap, 0);
            int _size = _config.GetNbrCanals();
            for (int i = 1; i < _size; ++i)
                _config.SetTextureAt(_validBitmap, i);

            // Assert
            Assert.Throws<TextureCombinerException>(() => _generator.GenerateBitmap());
        }

        [Test]
        public void GenerateBitmap_Config_MissingAlpha_ThrowsException()
        {
            BitmapGenerator _generator = new BitmapGenerator();
            BitmapConfig _config = BitmapConfig.Instance;
            BitmapSource _validBitmap = new WriteableBitmap(800, 800, 72, 72, PixelFormats.Bgra32, null);
            int _size = _config.GetNbrCanals();
            for (int i = 0; i < _size; ++i)
                _config.SetTextureAt(_validBitmap, i);

            _config.SetPixelFormat("BGRA32");

            // Assert
            Assert.Throws<TextureCombinerException>(() => _generator.GenerateBitmap());
        }
        #endregion
    }
}
