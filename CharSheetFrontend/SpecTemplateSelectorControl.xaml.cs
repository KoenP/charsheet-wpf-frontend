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
    /// Interaction logic for SpecTemplateSelectorControl.xaml
    /// </summary>
    public partial class SpecTemplateSelectorControl : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Fields.
        public static readonly DependencyProperty SpecControlArgsProperty =
            DependencyProperty.Register("SpecControlArgs", typeof(SpecControlArgs), typeof(SpecTemplateSelectorControl),
                new PropertyMetadata(null, OnSpecControlArgsChanged));

        public SpecControlArgs SpecControlArgs
        {
            get => (SpecControlArgs)GetValue(SpecControlArgsProperty);
            set => SetValue(SpecControlArgsProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Constructor.
        public SpecTemplateSelectorControl()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Methods.
        private static void OnSpecControlArgsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SpecTemplateSelectorControl)d;
            var newArgs = (SpecControlArgs)e.NewValue;
            control.OnSpecControlArgsChanged(newArgs);
        }
        protected void OnSpecControlArgsChanged(SpecControlArgs newArgs)
        {
            contentControl.Content = newArgs;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Events and event handlers.
        public static readonly RoutedEvent ChoiceEvent =
            EventManager.RegisterRoutedEvent("ChoiceEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(SpecTemplateSelectorControl));

        public event RoutedEventHandler Choice
        {
            add { AddHandler(ChoiceEvent, value); }
            remove { RemoveHandler(ChoiceEvent, value); }
        }

        // I'm not sure whether there is a more convenient way of bubbling up these events than manually re-raising them.
        private void SpecControl_Choice(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new ChoiceEventArgs(ChoiceEvent, (ChoiceEventArgs)e));
        }
    }
}
