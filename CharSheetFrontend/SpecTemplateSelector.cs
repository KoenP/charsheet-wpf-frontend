using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CharSheetFrontend
{
    public class SpecTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ListSpecTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            JToken root = ((JObject)item).Root;
            switch (root["spectype"].ToObject<string>())
            {
                case "list":
                    var specOptions = root["list"].ToObject<List<ListSpecOption>>();
                    return ListSpecTemplate;
                default:
                    return null;
            }
        }
    }
}
