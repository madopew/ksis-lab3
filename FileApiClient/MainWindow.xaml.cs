using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileApiClient.Annotations;

namespace FileApiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string currentPath;
        public string CurrentPath
        {
            get => currentPath;
            private set
            {
                currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
            }
        }
        
        public MainWindow()
        {
            CurrentPath = "/";
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}