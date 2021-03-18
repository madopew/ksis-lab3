using System;
using System.Windows.Controls;

namespace FileApiClient.Views
{
    public partial class FileContentView : UserControl
    {
        public string FileName { get; }
        private DateTime Date { get; }
        private long Size { get; }

        public string DateStr =>
            (Date == DateTime.MinValue) ? "" : $"{Date.ToShortTimeString()} {Date.ToShortDateString()}";
        public string SizeStr => (Size == -1) ? "<dir>" : Size.ToString();
        
        public FileContentView(string name, DateTime date, long size)
        {
            FileName = name;
            Date = date;
            Size = size;
            InitializeComponent();
        }
    }
}