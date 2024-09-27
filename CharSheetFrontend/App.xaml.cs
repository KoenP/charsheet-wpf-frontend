using System.Configuration;
using System.Data;
using System.Windows;
using System.Net.Http;
using System.Windows.Controls;

namespace CharSheetFrontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public HttpClient HttpClient = new()
        {
            BaseAddress = new Uri("http://localhost:8000"),
        };
    }
}
