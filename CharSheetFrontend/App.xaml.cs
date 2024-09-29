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
        public CharSheetHttpClient Client { get; } = new();
    }
}
