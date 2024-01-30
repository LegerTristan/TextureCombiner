using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TextureCombiner.UI.Controls
{
    /// <summary>
    /// Logique d'interaction pour TextureParameters.xaml
    /// </summary>
    public partial class TextureParameters : UserControl
    {
        BitmapConfig ConfigInstance => BitmapConfig.Instance;

        public TextureParameters()
        {
            InitializeComponent();
            SetupConfigFormats();
        }

        public void SetupConfigFormats()
        {
            ComboBoxItem _itemTexture = CbsTextureFormat.SelectedValue as ComboBoxItem,
                         _itemPixel = CbsPixelFormat.SelectedValue as ComboBoxItem;

            ConfigInstance.SetTextureFormat((string)_itemTexture.Content);
            ConfigInstance.SetPixelFormat((string)_itemPixel.Content);
        }

        void OnTextureFormatChanged(object _sender, SelectionChangedEventArgs _eventArgs)
        {
            ComboBoxItem _item = CbsTextureFormat.SelectedValue as ComboBoxItem;
            ConfigInstance.SetTextureFormat((string)_item.Content);
        }

        void OnPixelFormatChanged(object _sender, SelectionChangedEventArgs _eventArgs)
        {
            ComboBoxItem _item = CbsPixelFormat.SelectedValue as ComboBoxItem;
            ConfigInstance.SetPixelFormat((string)_item.Content);
        }

        void OnDesiredWidthUpdated(object _sender, RoutedEventArgs _eventArgs)
        {
            bool _result = int.TryParse(TxtBoxWidth.Text, out int _value);
            if (_result && _value >= 0 && _value <= BitmapConfig.MAX_SIZE)
                ConfigInstance.SetWidth(_value);
            else
                TxtBoxWidth.Text = ConfigInstance.Width.ToString();
        }

        void OnDesiredHeightUpdated(object _sender, RoutedEventArgs _eventArgs)
        {
            bool _result = int.TryParse(TxtBoxHeight.Text, out int _value);
            if (_result && _value >= 0 && _value <= BitmapConfig.MAX_SIZE)
                ConfigInstance.SetHeight(_value);
            else
                TxtBoxHeight.Text = ConfigInstance.Height.ToString();
        }
    }
}
