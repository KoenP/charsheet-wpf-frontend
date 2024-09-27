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
            JToken root = ((Option)item).Spec.Root;
            switch (root["spectype"].ToObject<string>())
            {
                case "list":
                    return ListSpecTemplate;
                ////case "from":
                //    return FromSpecTemplate;
                default:
                    return null;
            }
        }
    }
}
