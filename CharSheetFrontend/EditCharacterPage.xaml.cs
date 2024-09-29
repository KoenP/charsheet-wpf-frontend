using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CharSheetFrontend
{
    /// <summary>
    /// Interaction logic for EditCharacterPage.xaml
    /// </summary>
    public partial class EditCharacterPage : Page
    {
        private readonly Character _character;
        private readonly HttpClient _httpClient;
        private ObservableCollection<Tab> Tabs { get; set; } = [];

        public EditCharacterPage(HttpClient httpClient, Character character)
        {
            _character = character;
            _httpClient = httpClient;
            InitializeComponent();
            levelTabControl.ItemsSource = Tabs;
            LoadEditPageData();
            levelTabControl.SelectedIndex = Tabs.Count - 1;
        }

        private async void LoadEditPageData()
        {
            // TODO delete
            string editPageDataStr = await _httpClient.GetStringAsync($"api/character/{_character.CharId}/edit_character_page");
            EditPageData editPageData = JsonConvert.DeserializeObject<EditPageData>(editPageDataStr);


            // TODO error handling
            var newTabs = editPageData.Options.Keys
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
        }

        /// <summary>
        /// Group options by category, and order the categories by ascending category index.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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
                    // The interface is a bit slow; perhaps this contributes to that though.
                    // If I had time to properly profile the slow updates, this would be my first suspect.
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

        async private void SpecControl_Choice(object sender, RoutedEventArgs e)
        {
            // TODO type conversion error handling.
            ChoiceEventArgs args = ((ChoiceEventArgs)e);
            string uri = $"api/character/{_character.CharId}/choice?source={args.Origin}&id={args.Id}&choice={args.Choice}";
            HttpResponseMessage response = await _httpClient.PostAsync(uri, null);
            LoadEditPageData();
        }
    }

    public class EditPageData
    {
        public Dictionary<int, List<Option>> Options { get; set; }
    }

    public class OptionCategory
    {
        public string DisplayOriginCategory { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Id { get; set; }
        [JsonProperty("display_id")] public string DisplayId { get; set; }
        [JsonProperty("origin")] public string Origin { get; set; }
        [JsonProperty("origin_category")] public string OriginCategory { get; set; }
        [JsonProperty("display_origin_category")] public string DisplayOriginCategory { get; set; }
        [JsonProperty("origin_category_index")] public int OriginCategoryIndex { get; set; }

        /// <summary>
        ///  There are different kinds of Spec which need to be parsed differently, so we postpone parsing them.
        /// </summary>
        [JsonProperty("spec")] public JObject Spec { get; set; }

        /// <summary>
        ///  There are different kinds of Choice which need to be parsed differently (the structure depends on the Spec),
        ///  so we postpone parsing them.
        /// </summary>
        [JsonProperty("choice")] public JToken Choice { get; set; }

        public Func<string, string> MkChoice { get; set; } = s => s;

        public bool IsEnabled { get; set; } = true;
    }
}
