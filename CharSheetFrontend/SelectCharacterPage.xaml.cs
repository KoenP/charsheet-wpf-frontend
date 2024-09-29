using Newtonsoft.Json;
using System.Net.Http;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CharSheetFrontend
{
    /// <summary>
    /// Interaction logic for SelectCharacterPage.xaml
    /// </summary>
    public partial class SelectCharacterPage : Page
    {

        private readonly CharSheetHttpClient _client;

        public SelectCharacterPage(CharSheetHttpClient client)
        {
            InitializeComponent();
            _client = client;
            UpdateCharacterList();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Event handlers.
        private void SelectCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            Character character = (Character) ((Button)sender).Tag;
            NavigationService.Navigate(new EditCharacterPage(_client, character));
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
        private async void CreateNewCharacter(string name)
        {
            if (name.Length != 0)
            {
                string uuid = await _client.PostCreateCharacter(name);
                Character character = new Character() { CharId = uuid, Name = name };
                NavigationService.Navigate(new EditCharacterPage(_client, character));
            } else
            {
                MessageBox.Show("Please enter a non-empty name.");
            }
        }

        private async void UpdateCharacterList()
        {
            charList.ItemsSource = await _client.GetCharacterList();
        }
    }

}
