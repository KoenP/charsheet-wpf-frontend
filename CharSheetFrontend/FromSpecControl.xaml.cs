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
        public static readonly DependencyProperty SpecControlArgsProperty =
            DependencyProperty.Register("SpecControlArgs", typeof(SpecControlArgs), typeof(FromSpecControl),
                new PropertyMetadata(null, FromSpecControl.OnSpecControlArgsChanged));

        public SpecControlArgs SpecControlArgs
        {
            get => (SpecControlArgs)GetValue(SpecControlArgsProperty);
            set => SetValue(SpecControlArgsProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Constructor.
        public FromSpecControl()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Methods
        private static void OnSpecControlArgsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO shared superclass with ListSpecControl?
            var control = (FromSpecControl)d;
            var newSpecControlArgs = (SpecControlArgs)e.NewValue;
            control.OnSpecControlArgsChanged(newSpecControlArgs);
        }

        protected void OnSpecControlArgsChanged(SpecControlArgs newArgs)
        {
            switch (newArgs.Spec)
            {
                case Spec.FromSpec fromSpec:
                    itemsControl.Items.Clear();

                    // fromSpec.Num == null indicates that the number of options is
                    // unbounded (such as adding spells to wizard spellbook).
                    // In that case, the number of subcontrols we render is the number of 
                    // choices, plus one.
                    int num = fromSpec.Num ?? (newArgs.Choice.Count + 1);

                    for (int i = 0; i < num; i++)
                    {
                        SpecControlArgs subArgs = newArgs with {
                            // Select the relevant choice for the sub-spec.
                            Choice = i < newArgs.Choice.Count ? [newArgs.Choice[i]] : [],

                            // Pass through the sub-spec.
                            Spec = fromSpec.SubSpec,

                            // To fire a Choice event, the sub-spec control should insert it
                            // at the correct point in the list of choices.
                            MkChoiceFn = applyChoice(i, newArgs.Choice),

                            // We enable the subcontrols that have a registered choice,
                            // plus one blank one (if it exists).
                            IsEnabled = i <= newArgs.Choice.Count,

                            // Iff ths is a unique_from spec, disable the already-chosen values.
                            DisabledOptions = fromSpec.Unique ? newArgs.Choice : []
                        };
                        itemsControl.Items.Add(subArgs);
                    }
                    break;
            }
        }

        static private Func<string, string> applyChoice(int i, ImmutableList<string> choices)
        {
            return choice =>
            {
                var newChoices = i < choices.Count
                    ? choices.SetItem(i, choice)
                    : choices.Add(choice);
                return "[" + string.Join(", ", newChoices) + "]";
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

        // TODO I don't understand why I don't need to explicitly re-raise the event
        // here (doing so doubles the event), but I do need to re-raise it in
        // SpecTemplateSelectorControl
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
