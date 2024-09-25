using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private readonly Character character;
        private readonly HttpClient httpClient;

        public EditCharacterPage(HttpClient httpClient, Character character)
        {
            this.character = character;
            InitializeComponent();


            this.httpClient = httpClient;
            LoadEditPageData();
        }

        private async void LoadEditPageData()
        {
            string editPageDataStr = await httpClient.GetStringAsync($"api/character/{character.CharId}/edit_character_page");
            EditPageData editPageData = JsonConvert.DeserializeObject<EditPageData>(editPageDataStr);

            // int charLevel = editPageData.Options.Keys.Max(); // TODO error handling
            levelTabControl.DataContext = editPageData.Options.Keys
                .OrderDescending()
                .Select(lvl => new Tab {
                    Level = lvl,
                    OptionCategories = CategorizeOptions(editPageData.Options[lvl]),
                });
            levelTabControl.SelectedIndex = 1;
        }

        private void levelTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        static private List<OptionCategory> CategorizeOptions(IEnumerable<Option> options)
        {
            return options
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
        [JsonProperty("origin_category")] public string OriginCategory { get; set; }
        [JsonProperty("display_origin_category")] public string DisplayOriginCategory { get; set; }
        [JsonProperty("origin_category_index")] public int OriginCategoryIndex { get; set; }

        /// <summary>
        ///  There are different kinds of Spec which need to be parsed differently, so we postpone parsing them.
        /// </summary>
        [JsonProperty("spec")] public JObject Spec { get; set; }
    }
}
