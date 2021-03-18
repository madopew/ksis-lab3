using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FileApiClient.Annotations;
using FileApiClient.Models;
using FileApiClient.Views;

namespace FileApiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const string ApiUrl = "http://localhost:5000/api/fs";
        private string currentPath;
        public string CurrentPath
        {
            get => $"/{currentPath}";
            private set
            {
                currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
                UpdatePanel();
            }
        }

        private readonly HttpClient client = new HttpClient();
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CurrentPath = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdatePanel()
        {
            var requestUri = $"{ApiUrl}/list{CurrentPath}";
            var result = await client.GetAsync(requestUri);
            if (!result.IsSuccessStatusCode)
            {
                MessageBox.Show("Cannot list current directory", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FilesPanel.Children.Clear();
            var contents = await result.Content.ReadAsAsync<IEnumerable<DirectoryEntry>>();
            contents.ToList().ForEach(AddToPanel);
        }

        private void AddToPanel(DirectoryEntry entry)
        {
            var fContent = new FileContentView(entry, Entry_OnDoubleClick);
            FilesPanel.Children.Add(fContent);
        }

        private void UpdateButton_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CurrentPath = "";
        }

        private void Entry_OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is FileContentView view)
            {
                switch (view.Entry.Type)
                {
                    case DirectoryEntryType.Directory:
                        CurrentPath = view.Entry.Path;
                        break;
                    case DirectoryEntryType.File:
                        //TODO
                        break;
                }
            }
        }
    }
}