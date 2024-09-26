using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CharSheetFrontend
{
    /// <summary>
    /// Convert <c>Option</c> with spec of the form <c>{ "spectype": "list", "list": [...] }</c>
    /// into a list of pairs of <c>ListSpecOption</c> and a boolean indicating whether that option is the selected one.
    /// </summary>
    public class ListSpecConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"><c>Option</c> value</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <returns><c>ListSpec</c> value derived from the <c>value</c>'s <c>Spec</c> and, possibly, <c>Choice</c> fields</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Option option = (Option)value;
            string choice = option.Choice.ToObject<string>();
            return option.Spec.Root["list"]
                .ToObject<List<ListSpecOption>>()
                .Select(opt => new ListSpecOption() { Opt = opt.Opt, Selected = choice == opt.Opt }); // TODO Maybe just drop the ListSpecOption type
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListSpecOption : INotifyPropertyChanged
    {
        public string Opt { get; set; }
        // TODO desc
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // TODO understand this better
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
