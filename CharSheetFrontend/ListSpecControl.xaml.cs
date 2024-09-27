using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private readonly HttpClient _httpClient;
        private readonly Character _character;

        public static readonly DependencyProperty OptionProperty =
            DependencyProperty.Register("Option", typeof(Option), typeof(ListSpecControl),
                new PropertyMetadata(null, OnOptionChanged));

        public Option Option
        {
            get => (Option) GetValue(OptionProperty);
            set => SetValue(OptionProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Constructor.

        public ListSpecControl()
        {
            InitializeComponent();
            App app = ((App)Application.Current);
            _httpClient = app.HttpClient;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Methods.

        private static void OnOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ListSpecControl)d;
            var newOption = (Option)e.NewValue;
            control.OnOptionChanged(newOption);
        }

        protected void OnOptionChanged(Option newOption)
        {
            var listSpecOptions = newOption.Spec.Root["list"].ToObject<List<ListSpecOption>>();
            comboBox.Items.Clear();
            foreach (var listSpecOption in listSpecOptions)
            {
                comboBox.Items.Add(new ComboBoxItem()
                {
                    Content = listSpecOption.Opt,
                    IsSelected = listSpecOption.Opt == newOption.Choice.ToObject<string>()
                });
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
            RaiseEvent(new ChoiceEventArgs(ChoiceEvent, Option.Origin, Option.Id, choice));
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
