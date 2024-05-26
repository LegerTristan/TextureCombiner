using System;
using System.Diagnostics;
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
            ComboBoxItem _item = CbsWidth.SelectedValue as ComboBoxItem;
            if (_item != null)
                ConfigInstance.SetWidth(int.Parse(_item.Content.ToString()));
            else
            {
                Console.WriteLine("An error has occured !");
                Debugger.Break();
            }
        }

        void OnDesiredHeightUpdated(object _sender, RoutedEventArgs _eventArgs)
        {
            ComboBoxItem _item = CbsHeight.SelectedValue as ComboBoxItem;
            if (_item != null)
                ConfigInstance.SetHeight(int.Parse(_item.Content.ToString()));
            else
            {
                Console.WriteLine("An error has occured !");
                Debugger.Break();
            }
        }
    }
}
