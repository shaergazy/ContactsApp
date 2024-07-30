using ContactsApp.ViewModels;
using System.Windows;

namespace ContactsApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
