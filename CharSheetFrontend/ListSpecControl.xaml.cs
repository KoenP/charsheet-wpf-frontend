using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ListSpec.xaml
    /// </summary>
    public partial class ListSpecControl : UserControl
    {
        public static readonly DependencyProperty SpecProperty =
            DependencyProperty.Register("Spec", typeof(JObject), typeof(ListSpecControl));

        public JObject Spec
        {
            get { return (JObject)GetValue(SpecProperty); }
            set
            {
                SetValue(SpecProperty, value);
            }
        }

        public List<ListSpecOption> ListSpec
        {
            get
            {
                return Spec != null ? Spec.Root["list"].ToObject<List<ListSpecOption>>() : null;
            }
        }

        /*
        public static readonly DependencyProperty OptionProperty =
            DependencyProperty.Register("Option", typeof(Option), typeof(ListSpecControl), new PropertyMetadata(new Option()));

        public JObject Option
        {
            get { return (JObject)GetValue(OptionProperty); }
            set { SetValue(OptionProperty, value); }
        }
        */


        public ListSpecControl()
        {
            InitializeComponent();
            comboBox.DataContext = ListSpec;
            // this.listSpec = listSpec;
            // comboBox.ItemsSource = listSpec;
        }

    }

    // TODO move this somewhere sensible.
}
