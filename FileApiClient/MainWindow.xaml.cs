using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FileApiClient.Annotations;
using FileApiClient.Models;
using FileApiClient.Views;
using Microsoft.Win32;
using MimeTypes;
using Newtonsoft.Json;

namespace FileApiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const string ApiUrl = "http://localhost:5000/api/fs";
        private const string DownloadsFolder = "./Downloads";
        
        private string currentPath;
        public string CurrentPath
        {
            get => currentPath;
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
            var result = await client.GetAsync($"{ApiUrl}/list/{CurrentPath}");
            if (!result.IsSuccessStatusCode)
            {
                MessageBox.Show("Cannot list current directory", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentPath = "";
                return;
            }

            FilesPanel.Children.Clear();
            var contents = await result.Content.ReadAsAsync<IEnumerable<DirectoryEntry>>();
            contents.ToList().ForEach(AddToPanel);
        }

        private void AddToPanel(DirectoryEntry entry)
        {
            var fContent = new FileContentView(entry, Entry_OnDoubleClick, Entry_OnDelete);
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
                        SaveFile(view.Entry);
                        break;
                }
            }
        }

        private void SaveFile(DirectoryEntry file)
        {
            var downloadMsgRslt = MessageBox.Show($"Do you want to download file {file.Name}?", "Download",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (!Directory.Exists(DownloadsFolder))
            {
                Directory.CreateDirectory(DownloadsFolder);
            }

            if (downloadMsgRslt == MessageBoxResult.Yes)
            {
                var webClient = new WebClient();
                webClient.DownloadFile($"{ApiUrl}/download/{file.Path}", $"./Downloads/{file.Name}");
                MessageBox.Show("Download complete!", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start(Path.GetFullPath(DownloadsFolder));
            }
        }

        private async void CreateFolderButton_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var createDialog = new CreateFolderDialog();
            if (createDialog.ShowDialog() == true)
            {
                var name = createDialog.FolderName;
                var result = await client.PostAsync($"{ApiUrl}/create/{CurrentPath}?name={name}", null);
                if (result.IsSuccessStatusCode)
                {
                    UpdatePanel();
                }
                else
                {
                    var response = await result.Content.ReadAsStringAsync();
                    MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void UploadButton_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Choose a file",
                Filter = "Any file |*.*",
                Multiselect = false,
            };

            if (openDialog.ShowDialog() == true)
            {
                var info = new FileInfo(openDialog.FileName);
                FileUpload file;
                using (var fileStream = new FileStream(openDialog.FileName, FileMode.Open))
                {
                    using (var memStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memStream);
                        file = new FileUpload
                        {
                            ContentType = MimeTypeMap.GetMimeType(info.Extension),
                            Name = Path.GetFileNameWithoutExtension(info.Name),
                            FileBase64 = Convert.ToBase64String(memStream.ToArray()),
                        };
                    }
                }

                var payload = JsonConvert.SerializeObject(file);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var result = await client.PostAsync($"{ApiUrl}/upload/{CurrentPath}", content);

                if (result.IsSuccessStatusCode)
                {
                    UpdatePanel();
                }
                else
                {
                    var response = await result.Content.ReadAsStringAsync();
                    MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Entry_OnDelete(object sender, MouseButtonEventArgs e)
        {
            if (sender is FontAwesome.WPF.FontAwesome fa)
            {
                var view = (FileContentView)((StackPanel)fa.Parent).Parent;
                var deleteMsgResult = MessageBox.Show($"Do you want to delete {view.Entry.Name}?", "Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (deleteMsgResult == MessageBoxResult.Yes)
                {
                    var result = await client.DeleteAsync($"{ApiUrl}/delete/{view.Entry.Path}");

                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Deletion complete!", "Delete", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        UpdatePanel();
                    }
                    else
                    {
                        var response = await result.Content.ReadAsStringAsync();
                        MessageBox.Show(response, "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}