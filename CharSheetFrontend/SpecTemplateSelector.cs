using System.Windows;
using System.Windows.Controls;

namespace CharSheetFrontend
{
    public class SpecTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate ListSpecTemplate { get; set; }
        public DataTemplate FromSpecTemplate { get; set; }
        public DataTemplate UnimplementedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (((SpecControlArgs)item).Spec)
            {
                case Spec.ListSpec listSpec:
                    return ListSpecTemplate;
                case Spec.FromSpec fromSpec:
                    return FromSpecTemplate;
                default:
                    return UnimplementedTemplate;
            }
        }
    }
}
