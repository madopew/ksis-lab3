using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FileApiClient.Views
{
    public partial class CreateFolderDialog : Window
    {
        private readonly Regex alphaRegex = new Regex("[^a-zA-Z]+");
        
        public string FolderName { get; set; }
        public CreateFolderDialog()
        {
            InitializeComponent();
        }

        private void FolderNameTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = alphaRegex.IsMatch(e.Text);
        }

        private void CreateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FolderNameTextBox.Text))
            {
                DialogResult = true;
                FolderName = FolderNameTextBox.Text;
            }

            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}