using Newtonsoft.Json;
using System.Windows;
using System.Windows.Navigation;
using System.Text.Json;

namespace CharSheetFrontend
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var client = ((App)Application.Current).Client;
            NavigationService.Navigate(new SelectCharacterPage(client));
        }

    }
}