using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using Newtonsoft.Json;

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
            var httpClient = ((App)Application.Current).HttpClient;
            NavigationService.Navigate(new SelectCharacterPage(httpClient));
        }

    }

    public class Character
    {
        [JsonProperty("char_id")] public string CharId { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }
}