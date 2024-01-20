using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TextureCombiner.Source.Datas.Utils;


namespace TextureCombiner
{
    /// <summary>
    /// Logique d'interaction pour TextureChannelControl.xaml
    /// </summary>
    public partial class TextureChannelControl : UserControl
    {
        public static readonly DependencyProperty ChannelTextProperty =
            DependencyProperty.Register("ChannelText", typeof(string), typeof(TextureChannelControl), 
                new PropertyMetadata("R"));

        public static readonly DependencyProperty ForegroundTextProperty =
            DependencyProperty.Register("ForegroundText", typeof(Brush), typeof(TextureChannelControl),
                new PropertyMetadata(Brushes.Black));

        public event Action<BitmapImage> OnTextureImported = null;

        bool importedTexture = false,
             isEnabled = true;

        public string ChannelText
        {
            get { return (string)GetValue(ChannelTextProperty); }
            set { SetValue(ChannelTextProperty, value); }
        }

        public Brush ForegroundText
        {
            get { return (Brush)GetValue(ForegroundTextProperty); }
            set { SetValue(ForegroundTextProperty, value); }
        }

        public TextureChannelControl()
        {
            InitializeComponent();
            OnTextureImported += SetResolutionText;
        }

        #region WindowEvents
        void OnTextureChannelBtnClicked(object sender, RoutedEventArgs e)
        {
            if (!isEnabled)
                return;

            OpenFileDialog _dialog = new OpenFileDialog();
            _dialog.Filter = "JPG File|*.jpg|JPEG File|*.jpeg|PNG File|*.png";

            bool? _result = _dialog.ShowDialog();

            if (_result.HasValue && _result.Value)
                ImportTexture(_dialog.FileName);
        }

        void OnTextureChannelMouseDropped(object _sender, DragEventArgs _dragEvent)
        {
            if (!isEnabled)
                return;

            string[] _datas = (string[])_dragEvent.Data.GetData(DataFormats.FileDrop);
            if (_datas == null || _datas.Length == 0)
                return;

            ImportTexture(_datas[0]);
        }
        #endregion

        void ImportTexture(string _texturePath)
        {
            if (string.IsNullOrWhiteSpace(_texturePath))
                return;

            importedTexture = true;
            BitmapImage _img = Utils.LoadBitmapImage(_texturePath);
            TextureChannelImg.Source = _img;
            OnTextureImported?.Invoke(_img);
        }

        public void SetIsEnabled(bool _isEnabled)
        {
            isEnabled = _isEnabled;
            TextureChannelBtn.IsEnabled = _isEnabled;
        }

        void SetResolutionText(BitmapSource _src)
        {
            TextureChannelRes.Text = $"{_src.PixelWidth}x{_src.PixelHeight}";
            TextureChannelRes.Visibility = Visibility.Visible;
        }

        public bool IsValid()
        {
            return TextureChannelImg.Source != null && importedTexture;
        }
    }
}
