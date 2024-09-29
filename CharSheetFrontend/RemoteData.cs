using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharSheetFrontend
{
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
        public required string Id { get; set; }
        [JsonProperty("display_id")] public required string DisplayId { get; set; }
        [JsonProperty("origin")] public required string Origin { get; set; }
        [JsonProperty("origin_category")] public required string OriginCategory { get; set; }
        [JsonProperty("display_origin_category")] public required string DisplayOriginCategory { get; set; }
        [JsonProperty("origin_category_index")] public required int OriginCategoryIndex { get; set; }

        /// <summary>
        ///  There are different kinds of Spec which need to be parsed differently, so we postpone parsing them.
        /// </summary>
        [JsonProperty("spec")] public required JObject Spec { get; set; }

        /// <summary>
        ///  There are different kinds of Choice which need to be parsed differently (the structure depends on the Spec),
        ///  so we postpone parsing them.
        /// </summary>
        [JsonProperty("choice")] public required JToken Choice { get; set; }
    }
}
