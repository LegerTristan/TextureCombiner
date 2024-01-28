using System.Windows;
using System.Windows.Controls;

namespace TextureCombiner.UI.Controls
{
    /// <summary>
    /// Logique d'interaction pour AlphaTextureChannel.xaml
    /// </summary>
    public partial class AlphaTextureChannel : UserControl
    {
        const string WARNING_NOT_SUPPORT_ALPHA_BASE = " not support alpha channel !";
        const int SUPPORT_ALPHA_MASK = 0b10001;

        bool textureFormatSupportsAlpha = false, pixelFormatSupportsAlpha = false;
        bool UseAlpha => textureFormatSupportsAlpha && pixelFormatSupportsAlpha;

        public AlphaTextureChannel()
        {
            InitializeComponent();
            BindToBitmapConfig();
        }

        public bool IsAlphaChannelValid() => UseAlpha ? AlphaTextureChannelControl.IsValid() : true;

        public void BindToBitmapConfig()
        {
            BitmapConfig _config = BitmapConfig.Instance;

            _config.OnTextureFormatChanged += CheckAlphaChannelSupportFromTexture;
            _config.OnPixelFormatChanged += CheckAlphaChannelSupportFromPixel;
            CheckAlphaChannelSupportFromTexture(_config.TextureFormat);
            CheckAlphaChannelSupportFromPixel(_config.AuthorizedPixelFormat);
        }

        void CheckAlphaChannelSupportFromTexture(TextureFormat _format)
        {
            textureFormatSupportsAlpha = _format != TextureFormat.JPG;
            UpdateAlphaSupportWarning();
        }

        void CheckAlphaChannelSupportFromPixel(AuthorizedPixelFormat _format)
        {
            pixelFormatSupportsAlpha = ((int)_format & SUPPORT_ALPHA_MASK) != 0;
            UpdateAlphaSupportWarning();
        }

        void UpdateAlphaSupportWarning()
        {
            if(UseAlpha)
            {
                WarningAlphaIcon.Visibility = Visibility.Collapsed;
                WarningAlphaText.Visibility = Visibility.Collapsed;
                WarningAlphaTextureBorder.Visibility = Visibility.Collapsed;
                AlphaTextureChannelControl.SetIsEnabled(true);
            }
            else
            {
                FormatAlphaSupportWarningText();
                WarningAlphaIcon.Visibility = Visibility.Visible;
                WarningAlphaText.Visibility = Visibility.Visible;
                WarningAlphaTextureBorder.Visibility = Visibility.Visible;
                AlphaTextureChannelControl.SetIsEnabled(false);
            }
        }

        void FormatAlphaSupportWarningText()
        {
            bool _noneSupportAlpha = !textureFormatSupportsAlpha && !pixelFormatSupportsAlpha;
            string _textureFormatText = textureFormatSupportsAlpha ? "" : "TextureFormat ";
            string _andText = _noneSupportAlpha ? "and " : "";
            string _pixelFormatText = pixelFormatSupportsAlpha ? "" : "pixelFormat ";
            string _doText = _noneSupportAlpha ? "do" : "does";
            WarningAlphaText.Text = _textureFormatText + _andText + _pixelFormatText + _doText + 
                WARNING_NOT_SUPPORT_ALPHA_BASE;
        }
    }
}
