using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextureCombiner.UI.Controls
{
    /// <summary>
    /// Logique d'interaction pour EncodingParameters.xaml
    /// </summary>
    public partial class EncodingParameters : UserControl
    {
        TextureFormat compressionMask = TextureFormat.TGA | TextureFormat.TIFF;
        TextureFormat compressionLevelMask = TextureFormat.PNG | TextureFormat.TIFF;
        TextureFormat qualityMask = TextureFormat.JPG;

        public EncodingParameters()
        {
            InitializeComponent();
            BindToConfig();
        }

        public void BindToConfig()
        {
            BitmapConfig _config = BitmapConfig.Instance;
            _config.OnTextureFormatChanged += UpdateEncodingParameters;
            UpdateEncodingParameters(_config.TextureFormat);
        }

        void UpdateEncodingParameters(TextureFormat _format)
        {
            bool _isCompressionParameterVisible = (compressionMask & _format) != 0;
            TxtCompression.Visibility = _isCompressionParameterVisible ? Visibility.Visible : Visibility.Collapsed;
            CbsCompression.Visibility = _isCompressionParameterVisible ? Visibility.Visible : Visibility.Collapsed;

            bool _isCompressionLevelParameterVisible = (compressionLevelMask & _format) != 0;
            TxtCompressionLevel.Visibility = _isCompressionLevelParameterVisible ? Visibility.Visible : Visibility.Collapsed;
            SliderCompressionLevel.Visibility = _isCompressionLevelParameterVisible ? Visibility.Visible : Visibility.Collapsed;

            bool _isQualityParameterVisible = (qualityMask & _format) != 0;
            TxtQuality.Visibility = _isQualityParameterVisible ? Visibility.Visible : Visibility.Collapsed;
            SliderQuality.Visibility = _isQualityParameterVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
