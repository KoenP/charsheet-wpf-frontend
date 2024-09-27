using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public EditCharacterPage(HttpClient httpClient, Character character)
        {
            _character = character;
            _httpClient = httpClient;
            InitializeComponent();
            LoadEditPageData();
        }

        private async void LoadEditPageData()
        {
            string editPageDataStr = await _httpClient.GetStringAsync($"api/character/{_character.CharId}/edit_character_page");
            EditPageData editPageData = JsonConvert.DeserializeObject<EditPageData>(editPageDataStr);

            // int charLevel = editPageData.Options.Keys.Max(); // TODO error handling
            levelTabControl.DataContext = editPageData.Options.Keys
                .OrderDescending()
                .Select(lvl => new Tab {
                    Level = lvl,
                    OptionCategories = CategorizeOptions(editPageData.Options[lvl]),
                });
            levelTabControl.SelectedIndex = 2; // TODO
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
                    Options = grouping.ToList()
                })
                .ToList();
        }

        private class Tab
        {
            public int Level { get; set; }
            public List<OptionCategory> OptionCategories { get; set; }
        }

        async private void ListSpecControl_Choice(object sender, RoutedEventArgs e)
        {
            // TODO type conversion error handling.
            ChoiceEventArgs args = ((ChoiceEventArgs)e);
            string uri = $"api/character/{_character.CharId}/choice?source={args.Origin}&id={args.Id}&choice={args.Choice}";
            await _httpClient.PostAsync(uri, null);
            LoadEditPageData();
        }

        private void FromSpecControl_Choice(object sender, RoutedEventArgs e)
        {

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
    }
}
