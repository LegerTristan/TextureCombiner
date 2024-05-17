using System;
using System.Windows;
using System.Windows.Controls;

namespace TextureCombiner.UI.Controls
{
    /// <summary>
    /// Logique d'interaction pour EncodingParameters.xaml
    /// </summary>
    public partial class EncodingParameters : UserControl
    {
        public event Action<string> OnTGACompressionChanged = null,
                                    OnTIFFCompressionChanged = null;
        public event Action<double> OnQualityUpdated = null,
                                    OnCompressionLevelUpdated = null;

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
            CbsTGACompression.Visibility = TextureFormat.TGA == _format ? Visibility.Visible : Visibility.Collapsed;
            CbsTIFFCompression.Visibility = TextureFormat.TIFF == _format ? Visibility.Visible : Visibility.Collapsed;

            bool _isCompressionLevelParameterVisible = (compressionLevelMask & _format) != 0;
            TxtCompressionLevel.Visibility = _isCompressionLevelParameterVisible ? Visibility.Visible : Visibility.Collapsed;
            SliderCompressionLevel.Visibility = _isCompressionLevelParameterVisible ? Visibility.Visible : Visibility.Collapsed;

            bool _isQualityParameterVisible = (qualityMask & _format) != 0;
            TxtQuality.Visibility = _isQualityParameterVisible ? Visibility.Visible : Visibility.Collapsed;
            SliderQuality.Visibility = _isQualityParameterVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        void OnTGACompressionSelectionChanged(object _sender, SelectionChangedEventArgs _eventArgs)
        {
            ComboBoxItem _itemTGACompression = CbsTGACompression.SelectedValue as ComboBoxItem;
            OnTGACompressionChanged?.Invoke((string)_itemTGACompression.Content);
        }

        void OnTIFFCompressionSelectionChanged(object _sender, SelectionChangedEventArgs _eventArgs)
        {
            ComboBoxItem _itemTIFFCompression = CbsTIFFCompression.SelectedValue as ComboBoxItem;
            OnTIFFCompressionChanged?.Invoke((string)_itemTIFFCompression.Content);
        }

        void OnSliderCompressionLevelValueChanged(object _sender, RoutedPropertyChangedEventArgs<double> _eventArgs)
        {
            OnCompressionLevelUpdated?.Invoke(_eventArgs.NewValue);
        }

        void OnSliderQualityValueChanged(object _sender, RoutedPropertyChangedEventArgs<double> _eventArgs)
        {
            OnQualityUpdated?.Invoke(_eventArgs.NewValue);
        }
    }
}
