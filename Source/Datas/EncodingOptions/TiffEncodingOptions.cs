using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Tiff;
using TextureCombiner.Source.Datas.Utils;

namespace TextureCombiner.Source.Datas.EncodingOptions
{
    class TiffEncodingOptions : IEncodingOptions
    {
        public TextureFormat GetEncodedFormat() => TextureFormat.TIFF;

        public IImageEncoder GetEncoder()
        {
            bool _useAlpha = BitmapConfig.Instance.GetNbrCanals() == 4;
            TiffEncoder _encoder = new TiffEncoder();
            _encoder.BitsPerPixel = TiffBitsPerPixel.Bit24;
            return _encoder;
        }
    }
}
