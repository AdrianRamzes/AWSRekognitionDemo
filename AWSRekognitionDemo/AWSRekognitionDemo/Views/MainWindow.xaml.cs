using AWSRekognitionDemo.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace AWSRekognitionDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool _isRunning = true;

        private void v_Button_ToggleWebCam_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = !_isRunning;

            v_Button_ToggleWebCam.Content = _isRunning ? "Capture" : "Back";
        }
    }
}
