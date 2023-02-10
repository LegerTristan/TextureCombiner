using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

using UserControl = System.Windows.Controls.UserControl;

namespace TextureCombiner
{
    /// <summary>
    /// Interaction logic for ImportTexture.xaml
    /// </summary>
    public partial class ImportTexture : UserControl
    {
        public event Action<string> OnTextureImported = null;

        public ImportTexture()
        {
            InitializeComponent();
            OnTextureImported += (_texturePath) =>
            {
                ImgTexture.Source = new ImageSourceConverter().ConvertFromString(_texturePath) as ImageSource;
            };
        }

        private void OnImportTextureClicked(object _sender, RoutedEventArgs _e)
        {
            OpenFileDialog _dialog = new OpenFileDialog();
            _dialog.Filter = "JPG File|*.jpg|JPEG File|*.jpeg|PNG File|*.png";

            if (_dialog.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(_dialog.FileName))
                OnTextureImported?.Invoke(_dialog.FileName);
        }

        public bool IsValid()
        {
            return ImgTexture.Source != null;
        }
    }
}
