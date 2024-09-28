using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
    /// Interaction logic for FromSpecControl.xaml
    /// </summary>
    public partial class FromSpecControl : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Fields.
        public static readonly DependencyProperty OptionProperty =
            DependencyProperty.Register("Option", typeof(Option), typeof(FromSpecControl),
                new PropertyMetadata(null, FromSpecControl.OnOptionChanged));

        public Option Option
        {
            get => (Option)GetValue(OptionProperty);
            set => SetValue(OptionProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Constructor.
        public FromSpecControl()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Methods
        private static void OnOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO shared superclass with ListSpecControl?
            var control = (FromSpecControl)d;
            var newOption = (Option)e.NewValue;
            control.OnOptionChanged(newOption);
        }

        protected void OnOptionChanged(Option newOption)
        {
            // TODO probably don't need to do that.
            itemsControl.Items.Clear();
            var fromSpecOption = newOption.Spec.Root.ToObject<FromSpecOption>();

            // TODO handle "unlimited" case.
            int num = fromSpecOption.Num.ToObject<int>();

            // I took a shortcut here.
            // My web frontend handles arbitrary nesting of specs (at least I believe it does).
            // However in practice this doesn't occur. A from-spec always has a list-spec
            // as its sub-spec. So here I explicitly assume this to be the case, to save some
            // time on the implementation.
            ImmutableList<string> choices = ImmutableList
                .ToImmutableList(newOption.Choice.ToObject<List<string>>() ?? []);

            for (int i = 0; i < num; i++)
            {
                // TODO maybe change this datatype everywhere.
                Option option = new Option()
                { 
                    Choice = null,
                    Origin = newOption.Origin,
                    Id = newOption.Id,
                    Spec = fromSpecOption.Spec,
                    MkChoice = addToChoices(i, choices)
                };
                itemsControl.Items.Add(option);
            }
        }

        static private Func<string, string> addToChoices(int i, ImmutableList<string> choices)
        {
            return choice =>
            {
                var newChoices = i < choices.Count
                    ? choices.SetItem(i, choice)
                    : choices.Add(choice);
                return "[" + string.Join(",", newChoices) + "]";
            };
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Events and event handlers.
        public static readonly RoutedEvent ChoiceEvent =
            EventManager.RegisterRoutedEvent("ChoiceEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(FromSpecControl));

        public event RoutedEventHandler Choice
        {
            add { AddHandler(ChoiceEvent, value); }
            remove { RemoveHandler(ChoiceEvent, value); }
        }

        private void SpecTemplateSelectorControl_Choice(object sender, RoutedEventArgs e)
        {

            //RaiseEvent(new ChoiceEventArgs(ChoiceEvent, (ChoiceEventArgs)e));
        }
    }

    public class FromSpecOption
    {
        [JsonProperty("spectype")] public string SpecType { get; set; }
        public JObject Spec { get; set; }

        /// <summary>
        /// Can be either a number or the string "unlimited", so we postpone parsing.
        /// </summary>
        public JToken Num { get; set; }
    }
        
}
