using Newtonsoft.Json;
using System.Net.Http;
using System.Web;

namespace CharSheetFrontend
{
    public class CharSheetHttpClient
    {
        private HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://koen.vosjes.cloud"),
            //BaseAddress = new Uri("http://localhost:8000"),
        };

        public async Task<EditPageData> GetEditPageData(string charId)
        {
            string editPageDataStr = await httpClient
                .GetStringAsync($"api/character/{charId}/edit_character_page");
            return JsonConvert.DeserializeObject<EditPageData>(editPageDataStr);
        }

        public async Task<List<Character>> GetCharacterList()
        {
            string characters = await httpClient.GetStringAsync("api/list_characters");
            return JsonConvert.DeserializeObject<List<Character>>(characters);
        }

        public async Task PostChoice(string charId, ChoiceEventArgs args)
        {
            string uri = $"api/character/{charId}/choice?source={HttpUtility.UrlEncode(args.Origin)}&id={HttpUtility.UrlEncode(args.Id)}&choice={HttpUtility.UrlEncode(args.Choice)}";
            await httpClient.PostAsync(uri, null);
        }

        public async Task<string> PostCreateCharacter(string name)
        {
            HttpResponseMessage response = await httpClient.PostAsync($"api/create_character?name={HttpUtility.UrlEncode(name)}", null);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task PostGainLevel(string charId, string characterClass)
        {
            await httpClient.PostAsync($"api/character/{charId}/gain_level?class={characterClass}", null);
        }
    }
}
