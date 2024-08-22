using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ContactsApp.Views
{
    /// <summary>
    /// Interaction logic for TrackingWindow.xaml
    /// </summary>
    public partial class TrackingWindow : Window
    {
        public TrackingWindow()
        {
            InitializeComponent();
        }
        private void OpenUrl(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && !string.IsNullOrWhiteSpace(textBlock.Text))
            {
                var url = textBlock.Text;
                try
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
