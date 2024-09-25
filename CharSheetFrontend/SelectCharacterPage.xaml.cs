using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CharSheetFrontend
{
    /// <summary>
    /// Interaction logic for SelectCharacterPage.xaml
    /// </summary>
    public partial class SelectCharacterPage : Page
    {

        private readonly HttpClient httpClient;

        public SelectCharacterPage(HttpClient httpClient)
        {
            InitializeComponent();
            this.httpClient = httpClient;
            GetCharacterList();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Event handlers.
        private void SelectCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            Character character = (Character) ((Button)sender).Tag;
            NavigationService.Navigate(new EditCharacterPage(httpClient, character));
        }

        private void CreateCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewCharacter(newCharacterNameTextBox.Text);
        }

        private void NewCharacterNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) CreateNewCharacter(newCharacterNameTextBox.Text);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Private helper methods.

        // TODO return task?
        private async void CreateNewCharacter(string name)
        {
            if (name.Length != 0)
            {
                string uuid = await httpClient.GetStringAsync($"api/new_character?name={HttpUtility.UrlEncode(name)}");
                Character character = new Character() { CharId = uuid, Name = name };
                NavigationService.Navigate(new EditCharacterPage(httpClient, character));
            } else
            {
                MessageBox.Show("Please enter a non-empty name.");
            }
        }

        private async void GetCharacterList()
        {
            string characters = await httpClient.GetStringAsync("api/list_characters");
            charList.ItemsSource = JsonConvert.DeserializeObject<List<Character>>(characters);
        }
    }

}
