using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CharSheetFrontend
{
    public record SpecControlArgs(
        string Origin,
        string Id,
        Spec Spec,
        ImmutableList<string> Choice,
        Func<string,string> MkChoiceFn,
        bool IsEnabled
    )
    {
        public static SpecControlArgs FromOption(Option option)
        {
            return new SpecControlArgs(
                option.Origin,
                option.Id,
                Spec.FromJToken(option.Spec),
                JTokenToChoice(option.Choice),
                choice => choice,
                true
            );
        }

        private static ImmutableList<string> JTokenToChoice(JToken jsonChoice)
        {
            switch (jsonChoice)
            {
                case JArray array:
                    return array.Select(x => x.ToString()).ToImmutableList<string>();
                case JValue { Type: JTokenType.String } value:
                    return [value.ToString()];
                default:
                    return [];
            }
        }
    }

    public record Spec
    {
        public record ListSpec(List<string> Opts) : Spec();
        public record FromSpec(int Num, bool Unique, Spec SubSpec) : Spec();

        public static Spec FromJToken(JObject jsonSpec)
        {
            switch (jsonSpec.Root["spectype"].ToObject<string>())
            {
                case "list":
                    return new ListSpec(jsonSpec.Root["list"].ToObject<List<ListSpecOption>>()
                        .Select(opt => opt.Opt).ToList());
                case "from":
                    return new FromSpec(jsonSpec.Root["num"].ToObject<int>(),
                        false,
                        FromJToken(jsonSpec.Root["spec"].ToObject<JObject>())
                    );
                case "unique_from":
                    return new FromSpec(jsonSpec.Root["num"].ToObject<int>(),
                        true,
                        FromJToken(jsonSpec.Root["spec"].ToObject<JObject>())
                    );
                default:
                    return null; // TODO
            }
        }
    }

    public class OptionToSpecControlArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SpecControlArgs.FromOption((Option)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}   
