using System;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Threading;

using Path = System.IO.Path;
using Brushes = System.Windows.Media.Brushes;

namespace TextureCombiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string WINDOW_TITLE = "TextureCombiner";

        #region F/P
        public event Action<string> OnFolderPathChanged = null;
        public event Action<TextureFormat> OnTextureFormatUpdated = null;

        BitmapGenerator generator = new BitmapGenerator();
        BitmapConfig config;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Init all values at start of program.
        /// Bind UI elements to event Action.
        /// </summary>
        void Init()
        {
            ComboBoxItem _item = CbsFormat.SelectedValue as ComboBoxItem;
            config = new BitmapConfig(new string[4], config.StringToFormat((string)_item.Content));
            TxtBoxTextureName.Text = generator.DefaultFileName;
            TxtFolderPath.Text = generator.DefaultFolder;
            SliderQuality.Maximum = SliderQuality.Value = 100f;

            OnFolderPathChanged += (_newPath) =>
            {
                TxtFolderPath.Text = _newPath;
            };

            OnTextureFormatUpdated += SetCompressionVisibility;

            generator.OnGenerationProgress += (_value) =>
            {
                Dispatcher.Invoke(() => SetTitle($"Load : {_value}"));
            };

            generator.OnGenerationCompleted += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetTitle("TextureCombiner");
                    SetBitmapPreview();
                    SetEnableButtons(true);
                });
            };

            InitImportTexture();
        }

        /// <summary>
        /// Set title of the window
        /// </summary>
        /// <param name="_message">New title</param>
        void SetTitle(string _message)
        {
            Title = _message;
        }

        /// <summary>
        /// Init all Import Textures and bind their UI elements to OnTextureImported Action.
        /// </summary>
        void InitImportTexture()
        {
            ImportTextureA.TxtChannel.Text = "R";
            ImportTextureB.TxtChannel.Text = "G";
            ImportTextureC.TxtChannel.Text = "B";
            ImportTextureD.TxtChannel.Text = "A";

            ImportTextureA.TxtChannel.Foreground = Brushes.Red;
            ImportTextureB.TxtChannel.Foreground = Brushes.Green;
            ImportTextureC.TxtChannel.Foreground = Brushes.Blue;

            ImportTextureA.OnTextureImported += (_texturePath) => SetTexturePathsAt(0, _texturePath);
            ImportTextureB.OnTextureImported += (_texturePath) => SetTexturePathsAt(1, _texturePath);
            ImportTextureC.OnTextureImported += (_texturePath) => SetTexturePathsAt(2, _texturePath);
            ImportTextureD.OnTextureImported += (_texturePath) => SetTexturePathsAt(3, _texturePath);
        }

        /// <summary>
        /// Set visibility of compression options based on the <see cref="TextureFormat"/> selected.
        /// </summary>
        /// <param name="_format">Current <see cref="TextureFormat"/></param>
        void SetCompressionVisibility(TextureFormat _format)
        {
            Visibility _qualityVis = Visibility.Collapsed,
                       _compressionVisibility = Visibility.Visible,
                       _compTypeVisibility = Visibility.Collapsed;
            switch(_format)
            {
                case TextureFormat.JPG:
                    _qualityVis = Visibility.Visible;
                    break;
                case TextureFormat.PNG:
                    _compressionVisibility = Visibility.Collapsed;
                    break;
                case TextureFormat.TIFF:
                    _compTypeVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            TxtQuality.Visibility = _qualityVis;
            SliderQuality.Visibility = _qualityVis;

            TxtCompression.Visibility = _compressionVisibility;
            CbCompression.Visibility = _compressionVisibility;

            TxtCompType.Visibility = _compTypeVisibility;
            CbsCompType.Visibility = _compTypeVisibility;
        }

        /// <summary>
        /// Set a new texture path in the config at the index passed in param.
        /// </summary>
        /// <param name="_index">Index of the texture path</param>
        /// <param name="_texturePath">New texture path</param>
        void SetTexturePathsAt(int _index, string _texturePath)
        {
            config.TexturePaths[_index] = _texturePath;
            UpdateTexturePreview();
        }

        /// <summary>
        /// Update the preview texture if possible
        /// </summary>
        void UpdateTexturePreview()
        {
            if(!ImportTextureA.IsValid() || !ImportTextureB.IsValid() || !ImportTextureC.IsValid())
                return;

            if (config.UseAlpha && !ImportTextureD.IsValid())
                    return;

            SetEnableButtons(false);

            try
            {
                Thread _generateBitmapThr = new Thread(new ThreadStart(() => generator.GenerateBitmap(config)));
                _generateBitmapThr.Start();
            }
            catch (TextureCombinerException _exception)
            {
                DisplayError(_exception.Message);
            }
        }

        /// <summary>
        /// Set enable state of all buttons of the window
        /// </summary>
        /// <param name="_enable">Enable state</param>
        void SetEnableButtons(bool _enable)
        {
            ImportTextureA.BtnImportTexture.IsEnabled = _enable;
            ImportTextureB.BtnImportTexture.IsEnabled = _enable;
            ImportTextureC.BtnImportTexture.IsEnabled = _enable;
            ImportTextureD.BtnImportTexture.IsEnabled = _enable;
            BtnSave.IsEnabled = _enable;
            BtnBrowse.IsEnabled = _enable;
        }

        /// <summary>
        /// Set <see cref="Bitmap"/> preview based on the current packed <see cref="Bitmap"/>.
        /// </summary>
        void SetBitmapPreview()
        {
            BitmapSource _src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            generator.GeneratedBitmap.GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromWidthAndHeight(generator.GeneratedBitmap.Width, generator.GeneratedBitmap.Height));

            ImgPreview.Source = _src;
        }

        private void OnBtnSaveClicked(object _sender, RoutedEventArgs _e)
        {
            try
            {
                generator.SaveBitmap(Path.Combine(TxtFolderPath.Text, TxtBoxTextureName.Text) + config.FormatToString(config.Format),
                    TxtFolderPath.Text, config);
            }
            catch(TextureCombinerException _exception)
            {
                DisplayError(_exception.Message);
            }
        }

        /// <summary>
        /// Display an error in the TextBlock TxtError
        /// </summary>
        void DisplayError(string _error)
        {
            TxtError.Text = _error;
            TxtError.Visibility = Visibility.Visible;
        }

        private void OnBtnBrowseClicked(object _sender, RoutedEventArgs _e)
        {
            FolderBrowserDialog _dialog = new FolderBrowserDialog();

            if (_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OnFolderPathChanged?.Invoke(_dialog.SelectedPath);
        }

        private void OnAlphaClicked(object _sender, RoutedEventArgs _e)
        {
            if (CbAlpha.IsChecked == true)
            {
                ImportTextureD.Visibility = Visibility.Visible;
                config.UseAlpha = true;
            }
            else
            {
                ImportTextureD.Visibility = Visibility.Collapsed;
                config.UseAlpha = false;
            }
        }

        private void OnFormatChanged(object _sender, SelectionChangedEventArgs _e)
        {
            ComboBoxItem  _item = CbsFormat.SelectedValue as ComboBoxItem;
            config.Format = config.StringToFormat((string)_item.Content);

            OnTextureFormatUpdated?.Invoke(config.Format);
        }

        /// <summary>
        /// Clamp a value between an exclusive min and an exclusive max.
        /// </summary>
        /// <param name="_value">Value to clamp</param>
        /// <param name="_min">Min value</param>
        /// <param name="_max">Max value</param>
        /// <returns>Value clamped between min and max</returns>
        int Clamp(int _value, int _min, int _max)
        {
            _value = _value < _min ? _min : _value;
            _value = _value > _max ? _max : _value;

            return _value;
        }
        #endregion

        private void OnQualityChanged(object _sender, RoutedPropertyChangedEventArgs<double> _e)
        {
            config.CompressQuality = (int)SliderQuality.Value;
        }

        private void OnCompressionClicked(object _sender, RoutedEventArgs _e)
        {
            if (CbCompression.IsChecked == true)
            {
                SliderQuality.IsEnabled = true;
                config.UseCompression = true;
            }
            else
            {
                SliderQuality.IsEnabled = false;
                config.UseCompression = false;
            }
        }

        private void OnCompressionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            ComboBoxItem _item = CbsCompType.SelectedValue as ComboBoxItem;
            config.CompressionType = config.CompressionTypeFromString((string)_item.Content);
        }
    }
}
