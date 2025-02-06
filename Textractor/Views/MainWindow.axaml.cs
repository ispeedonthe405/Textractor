using Avalonia.Controls;

namespace Textractor.Views
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
        }
    }
}