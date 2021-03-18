using System;
using System.Windows.Controls;
using System.Windows.Input;
using FileApiClient.Models;

namespace FileApiClient.Views
{
    public partial class FileContentView
    {
        private Action<object, MouseButtonEventArgs> doubleClick;
        public DirectoryEntry Entry { get; set; }
        public string FileName => Entry.Name;
        public string DateStr =>
            (Entry.LastModified == DateTime.MinValue) ? "" : $"{Entry.LastModified.ToShortTimeString()} " +
                                                             $"{Entry.LastModified.ToShortDateString()}";
        public string SizeStr => (Entry.Size == -1) ? "<dir>" : Entry.Size.ToString();

        public FileContentView(DirectoryEntry entry, Action<object, MouseButtonEventArgs> onDoubleClick)
        {
            Entry = entry;
            doubleClick = onDoubleClick;
            InitializeComponent();
        }

        private void FileContentView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            doubleClick(sender, e);
        }
    }
}