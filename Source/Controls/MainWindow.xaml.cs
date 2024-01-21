﻿using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

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
                     ICON_ERROR_PATH = "Assets/IconError.png",
                     ICON_WARNING_PATH = "Assets/IconWarning.png";


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
            TxtBoxTextureName.Text = generator.DefaultFileName;
            TxtFolderPath.Text = generator.DefaultFolder;

            OnFolderPathChanged += SetFolderPathText; 
            config.OnPixelFormatChanged += UpdateAlphaChannel;
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

            UpdateAlphaChannel(AuthorizedPixelFormat.BGR24);
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
                Save();
        }

        /// <summary>
        /// Init all Import Textures and bind their UI elements to OnTextureImported Action.
        /// </summary>
        void InitImportTexture()
        { 
            TextureChannelA.OnTextureImported += (_texturePath) => SetTexturePathsAt(0, _texturePath);
            TextureChannelB.OnTextureImported += (_texturePath) => SetTexturePathsAt(1, _texturePath);
            TextureChannelC.OnTextureImported += (_texturePath) => SetTexturePathsAt(2, _texturePath);
            TextureChannelD.OnTextureImported += (_texturePath) => SetTexturePathsAt(3, _texturePath);
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

            if (config.UseAlpha() && !TextureChannelD.IsValid())
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
            TextureChannelD.SetIsEnabled(_enable);
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

        void Save()
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

        void UpdateAlphaChannel(AuthorizedPixelFormat _format)
        {
            bool _useAlpha = config.UseAlpha();
            WarningAlphaTextureBorder.Visibility = _useAlpha ? Visibility.Collapsed : Visibility.Visible;
            WarningAlphaTextureText.Visibility = _useAlpha ? Visibility.Collapsed : Visibility.Visible;
            TextureChannelD.SetIsEnabled(_useAlpha);
        }

        void SetFolderPathText(string _text) => TxtFolderPath.Text = _text;
        #endregion
    }
}
