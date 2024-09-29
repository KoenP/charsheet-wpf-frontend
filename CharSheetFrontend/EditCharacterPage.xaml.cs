using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace CharSheetFrontend
{
    /// <summary>
    /// Interaction logic for EditCharacterPage.xaml
    /// </summary>
    public partial class EditCharacterPage : Page
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Fields.
        private readonly Character _character;
        private readonly CharSheetHttpClient _client;
        private ObservableCollection<Tab> Tabs = [];

        ////////////////////////////////////////////////////////////////////////////////
        // Constructor.
        public EditCharacterPage(CharSheetHttpClient client, Character character)
        {
            _character = character;
            _client = client;
            InitializeComponent();
            levelTabControl.ItemsSource = Tabs;
            LoadEditPageData();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Private helper methods.
        private async Task LoadEditPageData()
        {
            // Fetch and parse remote data.
            // I took a bit of a shortcut here and did no proper error handling of JSON parsing.
            // Note: I should be disabling all inputs until the response arrives and is rendered.
            // I didn't get around to implementing that.
            EditPageData editPageData = await _client.GetEditPageData(_character.CharId);

            var newTabs = editPageData.Options.Keys
                // Highest level is the first (top) tab.
                .OrderDescending()
                .Select(lvl => new Tab
                {
                    Level = lvl,
                    OptionCategories = CategorizeOptions(editPageData.Options[lvl]),
                });
            UpdateTabs(newTabs);
        }

        private void UpdateTabs(IEnumerable<Tab> newTabs)
        {
            // If there are fewer new tabs than existing tabs, delete until they match.
            // The tabs are ordered by descending level, so this will always remove the
            // highest levels.
            for (int i = newTabs.Count(); i < Tabs.Count; i++)
            {
                Tabs.RemoveAt(0);
            }

            // If there are more new tabs than current tabs, create extra tabs.
            // As the tabs are ordered descending, the first new tabs in the sequence
            // are the ones that need to be added.
            foreach (var newTab in newTabs.Take(newTabs.Count() - Tabs.Count).Reverse())
            {
                Tabs.Insert(0, newTab);
            }

            // Update the remaining tabs.
            foreach ((var tab, var newTab) in Tabs.Zip(newTabs.TakeLast(Tabs.Count)))
            {
                tab.OptionCategories = newTab.OptionCategories;
            }

            // If no tab is selected, select the first one (highest level).
            levelTabControl.SelectedIndex = int.Max(0, levelTabControl.SelectedIndex);
        }

        private async Task GainLevelAs(string characterClass)
        {
            await _client.PostGainLevel(_character.CharId, characterClass);
            levelTabControl.SelectedIndex = -1;
            await LoadEditPageData();
        }

        /// <summary>
        ///  Group options by category, and order the categories by ascending category index.
        /// </summary>
        static private List<OptionCategory> CategorizeOptions(IEnumerable<Option> options)
        {
            return options
                .OrderBy(o => o.OriginCategoryIndex)
                .GroupBy(o => o.DisplayOriginCategory)
                .Select(grouping => new OptionCategory() {
                    DisplayOriginCategory = grouping.Key,
                    Options = [.. grouping]
                })
                .ToList();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Event handlers.

        /// <summary>
        ///  Submit the choice to the server and update the page.
        /// </summary>
        async private void SpecControl_Choice(object sender, RoutedEventArgs e)
        {
            ChoiceEventArgs args = ((ChoiceEventArgs)e);
            await _client.PostChoice(_character.CharId, args);
            LoadEditPageData();
        }
        private async void LevelUpComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We clear the combobox at the end of this method, so we need to make sure that doesn't
            // cause an infinite recursion.
            if (e.AddedItems.Count == 1)
            {
                string characterClass = (string)(((ComboBoxItem)e.AddedItems[0]).Content);
                await GainLevelAs(characterClass);
                ((ComboBox)sender).SelectedIndex = -1;
            }
        }

        /// <summary>
        ///  Keeps track of the options per character level.
        /// </summary>
        private class Tab : INotifyPropertyChanged
        {
            public int Level { get; set; }
            private List<OptionCategory> optionCategories;
            public List<OptionCategory> OptionCategories
            {
                get { return optionCategories; }
                set
                {
                    optionCategories = value;

                    // We don't bother actually checking if the tab got modified.
                    // The contents are re-generated from scratch anyway.
                    NotifyPropertyChanged("OptionCategories");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }
        }

    }
}
