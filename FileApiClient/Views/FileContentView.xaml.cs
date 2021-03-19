using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileApiClient.Models;

namespace FileApiClient.Views
{
    public partial class FileContentView
    {
        private readonly Action<object, MouseButtonEventArgs> doubleClick;
        private readonly Action<object, MouseButtonEventArgs> deleteClick;
        
        public DirectoryEntry Entry { get; set; }
        public string FileName => Entry.Name;
        public string DateStr =>
            (Entry.LastModified == DateTime.MinValue) ? "" : $"{Entry.LastModified.ToShortTimeString()} " +
                                                             $"{Entry.LastModified.ToShortDateString()}";
        public string SizeStr => (Entry.Size == -1) ? "<dir>" : Entry.Size.ToString();

        public FileContentView(DirectoryEntry entry, Action<object, MouseButtonEventArgs> onDoubleClick,
            Action<object, MouseButtonEventArgs> onDelete)
        {
            Entry = entry;
            doubleClick = onDoubleClick;
            deleteClick = onDelete;
            InitializeComponent();

            if (entry.Name == "..")
            {
                DeleteButton.Visibility = Visibility.Hidden;
            }
        }

        private void FileContentView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            doubleClick(sender, e);
        }

        private void DeleteButton_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            deleteClick(sender, e);
        }
    }
}