using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
    public partial class ListSpecControl : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Fields.
        public static readonly DependencyProperty SpecControlArgsProperty =
            DependencyProperty.Register("SpecControlArgs", typeof(SpecControlArgs), typeof(ListSpecControl),
                new PropertyMetadata(null, OnSpecControlArgsChanged));

        public SpecControlArgs SpecControlArgs
        {
            get => (SpecControlArgs) GetValue(SpecControlArgsProperty);
            set => SetValue(SpecControlArgsProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Constructor.
        public ListSpecControl()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Methods.
        private static void OnSpecControlArgsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ListSpecControl)d;
            var newArgs = (SpecControlArgs)e.NewValue;
            control.OnSpecControlArgsChanged(newArgs);
        }

        protected void OnSpecControlArgsChanged(SpecControlArgs newArgs)
        {
            switch (newArgs.Spec)
            {
                case Spec.ListSpec listSpec:
                    comboBox.Items.Clear();
                    comboBox.IsEnabled = newArgs.IsEnabled;

                    // Hack: In my web frontend the chosen element does not need to occur
                    // in the dropdown options, but for the combo box to work here it
                    // does. So if our choice does not occur in the list of options,
                    // we add a disabled option to the top of the dropdown.
                    var ensureChoiceIsOpt = newArgs.Choice.Except(listSpec.Opts)
                        .Select(opt => new ComboBoxItem()
                        {
                            Content = opt,
                            IsSelected = true,
                            IsEnabled = false
                        });

                    // Normal combo box items.
                    var regularItems = listSpec.Opts
                        .Select(opt => new ComboBoxItem()
                        {
                            Content = opt,
                            IsSelected = newArgs.Choice.Contains(opt)
                        });

                    foreach (var item in ensureChoiceIsOpt.Concat(regularItems))
                    {
                        comboBox.Items.Add(item);
                    }
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Events and event handlers.
        public static readonly RoutedEvent ChoiceEvent =
            EventManager.RegisterRoutedEvent("ChoiceEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(ListSpecControl));

        public event RoutedEventHandler Choice
        {
            add { AddHandler(ChoiceEvent, value); }
            remove { RemoveHandler(ChoiceEvent, value); }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string choice = (string)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content;
            ImmutableList<string> newChoice = SpecControlArgs.MkChoiceFn(choice);
            RaiseEvent(new ChoiceEventArgs(ChoiceEvent, SpecControlArgs.Origin, SpecControlArgs.Id,
                newChoice));
            // TODO error handling
        }
    }

    // TODO move class
    public class ListSpecOption
    {
        public string Opt { get; set; }
        // TODO desc
    }
}
