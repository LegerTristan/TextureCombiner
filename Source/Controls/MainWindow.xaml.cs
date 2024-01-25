using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;

namespace TextureCombiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string WINDOW_TITLE = "TextureCombiner",
                     INFO_GENERATION = "Generating the bitmap, please wait.",
                     INFO_GENERATION_SUCCESS = "Successfully generate the texture !",
                     INFO_SAVE = "Saving the texture into the specified folder, please wait.",
                     INFO_SAVE_SUCCESS = "Successfully saved the texture !",
                     WARNING_SIZE_EXCEED = "Desired size exceed textures size ! It will cause some loss of quality.",
                     ABOUT_TEXT = ", Tristan LEGER.\nMade with WPF.\nVersion 1.1",
                     ICON_ERROR_PATH = "Assets/IconError.png",
                     ICON_WARNING_PATH = "Assets/IconWarning.png",
                     DOCUMENTATION_LINK = "https://1drv.ms/b/s!AjthLxKQGnIl1mVqrURPnB7aoRqo?e=eQoA5E";
        const int RELEASE_YEAR = 2024;

        #region F/P
        public event Action<string> OnFolderPathChanged = null;

        DispatcherTimer timerTick;

        BitmapGenerator generator = new BitmapGenerator();
        BitmapIO io = new BitmapIO();
        BitmapConfig config;

        bool pendingGenerate = false,
             pendingSave = false;
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
            config = new BitmapConfig(new BitmapImage[4], (string)_item.Content, (string)_item.Content);
            TxtBoxTextureName.Text = io.DefaultFileName;
            TxtFolderPath.Text = io.DefaultFolder;

            OnFolderPathChanged += SetFolderPathText; 
            config.OnSizeExceed += DisplayWarningSizeExceed;

            generator.OnGenerationCompleted += (_bitmap) =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetTitle(WINDOW_TITLE);
                    _bitmap.Freeze();
                    SetBitmapPreview(_bitmap);
                    ComboBoxItem _pixelFormatItem = CbsPixelFormat.SelectedValue as ComboBoxItem;
                    config.SetBitmapPixelFormat((string)_pixelFormatItem.Content);;
                    SetEnableButtons(true);
                });
            };

            TextureChannelD.BindToBitmapConfig(config);
            InitImportTexture();
            StartTick();
        }

        /// <summary>
        /// Set title of the window
        /// </summary>
        /// <param name="_message">New title</param>
        void SetTitle(string _message)
        {
            Title = _message;
        }

        void StartTick()
        {
            timerTick = new DispatcherTimer();
            timerTick.Interval = TimeSpan.FromSeconds(1);
            timerTick.Tick += Tick;

            timerTick.Start();
        }

        void Tick(object _sender, EventArgs _eventArgs)
        {
            if (pendingGenerate)
                CallGenerator();

            if (pendingSave)
                CallSave();
        }

        /// <summary>
        /// Init all Import Textures and bind their UI elements to OnTextureImported Action.
        /// </summary>
        void InitImportTexture()
        { 
            TextureChannelA.OnTextureImported += (_texturePath) => SetTexturePathsAt(0, _texturePath);
            TextureChannelB.OnTextureImported += (_texturePath) => SetTexturePathsAt(1, _texturePath);
            TextureChannelC.OnTextureImported += (_texturePath) => SetTexturePathsAt(2, _texturePath);
            TextureChannelD.AlphaTextureChannelControl.OnTextureImported += (_texturePath) => SetTexturePathsAt(3, _texturePath);
        }

        /// <summary>
        /// Set a new texture path in the config at the index passed in param.
        /// </summary>
        /// <param name="_index">Index of the texture path</param>
        /// <param name="_texturePath">New texture path</param>
        void SetTexturePathsAt(int _index, BitmapImage _image)
        {
            config.SetTextureAt(_image, _index);
            UpdateTexturePreview();
        }

        /// <summary>
        /// Update the preview texture if possible
        /// </summary>
        void UpdateTexturePreview()
        {
            if(!TextureChannelA.IsValid() || !TextureChannelB.IsValid() || !TextureChannelC.IsValid())
                return;

            if (!TextureChannelD.IsAlphaChannelValid())
                return;

            SetEnableButtons(false);
            DisplayLog(INFO_GENERATION, Colors.Black);
            pendingGenerate = true;
        }

        void CallGenerator()
        {
            try
            {
                generator.GenerateBitmap(config);
                DisplayLog(INFO_GENERATION_SUCCESS, Colors.Green);
            }
            catch (Exception _exception)
            {
                DisplayError(_exception.Message);
            }
            finally
            {
                pendingGenerate = false;
            }
        }

        /// <summary>
        /// Set enable state of all buttons of the window
        /// </summary>
        /// <param name="_enable">Enable state</param>
        void SetEnableButtons(bool _enable)
        {
            TextureChannelA.SetIsEnabled(_enable);
            TextureChannelB.SetIsEnabled(_enable);
            TextureChannelC.SetIsEnabled(_enable);
            TextureChannelD.AlphaTextureChannelControl.SetIsEnabled(_enable);
            BtnSave.IsEnabled = _enable;
            BtnBrowse.IsEnabled = _enable;
        }

        /// <summary>
        /// Set <see cref="Bitmap"/> preview based on the current packed <see cref="Bitmap"/>.
        /// </summary>
        void SetBitmapPreview(BitmapSource _bitmap)
        {
            ImgPreview.Source = _bitmap;
        }

        private void OnBtnSaveClicked(object _sender, RoutedEventArgs _e)
        {
            DisplayLog(INFO_SAVE, Colors.Black);
            pendingSave = true;
        }

        void CallSave()
        {
            try
            {
                io.SaveBitmap(generator.GeneratedBitmap, config,
                    Path.Combine(TxtFolderPath.Text, TxtBoxTextureName.Text) + config.TextureFormatToString(),
                    TxtFolderPath.Text);
                DisplayLog(INFO_SAVE_SUCCESS, Colors.Green);
            }
            catch (TextureCombinerException _exception)
            {
                DisplayError(_exception.Message);
            }
            finally
            {
                pendingSave = false;
            }
        }

        /// <summary>
        /// Display an error in the TextBlock TxtError
        /// Hide TxtInfo if it was previously visible
        /// </summary>
        void DisplayError(string _error)
        {
            DisplayInfo(_error, Colors.Red);
            ImgIconInfo.Visibility = Visibility.Visible;
            Uri _tempUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ICON_ERROR_PATH));
            ImgIconInfo.Source = new BitmapImage(_tempUri);
        }

        void DisplayWarning(string _warning)
        {
            DisplayInfo(_warning, Colors.Orange);
            ImgIconInfo.Visibility = Visibility.Visible;
            Uri _tempUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ICON_WARNING_PATH));
            ImgIconInfo.Source = new BitmapImage(_tempUri);
        }

        /// <summary>
        /// Display an information in the TextBlock TxtInfo
        /// Hide TxtError if it was previously visible
        /// </summary>
        void DisplayLog(string _log, Color _color)
        {
            DisplayInfo(_log, _color);
            ImgIconInfo.Visibility = Visibility.Collapsed;

        }

        void OnAboutClicked(object _sender, RoutedEventArgs _eventsArgs)
        {
            int _year = DateTime.Now.Year;
            string _currentYear = RELEASE_YEAR == _year ? "" : "- " + _year.ToString();
            string _finalAbout = RELEASE_YEAR.ToString() + _currentYear + ABOUT_TEXT;
            System.Windows.MessageBox.Show(_finalAbout, "About", MessageBoxButton.OK);
        }

        void OnDocumentationClicked(object _sender, RoutedEventArgs _eventsArgs) => Process.Start(DOCUMENTATION_LINK);

        void DisplayInfo(string _info, Color _color)
        {
            TxtInfo.Text = _info;
            TxtInfo.Foreground = new SolidColorBrush(_color);
            TxtInfo.Visibility = Visibility.Visible;
        }

        void OnBtnBrowseClicked(object _sender, RoutedEventArgs _e)
        {
            FolderBrowserDialog _dialog = new FolderBrowserDialog();

            if (_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OnFolderPathChanged?.Invoke(_dialog.SelectedPath);
        }

        void OnTextureFormatChanged(object _sender, SelectionChangedEventArgs _eventArgs)
        {
            if (config == null)
                return;

            ComboBoxItem _item = CbsFormat.SelectedValue as ComboBoxItem;
            config.SetTextureFormat((string)_item.Content);
        }

        void OnPixelFormatChanged(object _sender, SelectionChangedEventArgs _eventArgs)
        {
            if (config == null)
                return;

            ComboBoxItem _item = CbsPixelFormat.SelectedValue as ComboBoxItem;
            config.SetPixelFormat((string)_item.Content);
        }

        void OnDesiredWidthUpdated(object _sender, RoutedEventArgs _eventArgs)
        {
            bool _result = int.TryParse(TxtBoxWidth.Text, out int _value);
            if (_result && _value >= 0)
                config.SetWidth(_value);
            else
                TxtBoxWidth.Text = config.Width.ToString();
        }

        void OnDesiredHeightUpdated(object _sender, RoutedEventArgs _eventArgs)
        {
            bool _result = int.TryParse(TxtBoxHeight.Text, out int _value);
            if (_result && _value >= 0)
                config.SetHeight(_value);
            else
                TxtBoxHeight.Text = config.Height.ToString();
        }

        void DisplayWarningSizeExceed() => DisplayWarning(WARNING_SIZE_EXCEED);

        void SetFolderPathText(string _text) => TxtFolderPath.Text = _text;
        #endregion
    }
}
