using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextureCombiner.UI.Controls
{
    /// <summary>
    /// Logique d'interaction pour FileParameters.xaml
    /// </summary>
    public partial class FileParameters : System.Windows.Controls.UserControl
    {
        readonly char[] EXCLUDE_CHARACTERS = new char[]
        {
            '/',
            '*',
            '<',
            '>',
            '\\',
            '?',
            '|',
            ':',
            '"',
        };

        public event Action<string> OnFolderPathChanged = null;

        string currentFileName = null;

        public string FileName => currentFileName;

        public FileParameters()
        {
            InitializeComponent();
            OnFolderPathChanged += SetFolderPathText; 
        }

        public void InitTexts(string _defaultFolder, string _defaultName)
        {
            TxtFolderPath.Text = _defaultFolder;
            TxtBoxTextureName.Text = currentFileName = _defaultName;
        }

        void OnBtnBrowseClicked(object _sender, RoutedEventArgs _e)
        {
            FolderBrowserDialog _dialog = new FolderBrowserDialog();

            if (_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OnFolderPathChanged?.Invoke(_dialog.SelectedPath);
        }

        void OnNameUpdated(object _sender, RoutedEventArgs _eventArgs)
        {
            string _text = TxtBoxTextureName.Text;
            foreach(char _character in _text)
            {
                if (EXCLUDE_CHARACTERS.Contains(_character))
                {
                    TxtBoxTextureName.Text = currentFileName;
                    return;
                }
            }

            currentFileName = TxtBoxTextureName.Text;
        }

        void SetFolderPathText(string _text) => TxtFolderPath.Text = _text;
    }
}
