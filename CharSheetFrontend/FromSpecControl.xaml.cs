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
                    // TODO probably don't need to do that.
                    itemsControl.Items.Clear();

                    // I took a shortcut here.
                    // My web frontend handles arbitrary nesting of specs (at least I believe it does).
                    // However in practice this doesn't occur. A from-spec always has a list-spec
                    // as its sub-spec. So here I explicitly assume this to be the case, to save some
                    // time on the implementation.
                    for (int i = 0; i < fromSpec.Num; i++)
                    {
                        // TODO maybe change this datatype everywhere.
                        SpecControlArgs subArgs = newArgs with {
                            Choice = i < newArgs.Choice.Count ? [newArgs.Choice[i]] : [],
                            Spec = fromSpec.SubSpec,
                            MkChoiceFn = applyChoice(i, newArgs.Choice),
                            IsEnabled = i <= newArgs.Choice.Count
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
