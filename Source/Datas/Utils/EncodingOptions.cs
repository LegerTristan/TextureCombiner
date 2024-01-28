using SixLabors.ImageSharp.Formats;

namespace TextureCombiner.Source.Datas.Utils
{
    interface IEncodingOptions
    {
        IImageEncoder GetEncoder();

        TextureFormat GetEncodedFormat();
    }
}
