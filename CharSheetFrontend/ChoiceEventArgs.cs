using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CharSheetFrontend
{
    public class ChoiceEventArgs : RoutedEventArgs
    {
        public readonly string Origin;
        public readonly string Id;
        public readonly string Choice;

        public ChoiceEventArgs(RoutedEvent routedEvent, string origin, string id, string choice)
            : base(routedEvent)
        {
            Origin = origin;
            Id = id;
            Choice = choice;
        }

        public ChoiceEventArgs(RoutedEvent routedEvent, ChoiceEventArgs args)
            : base(routedEvent)
        {
            Origin = args.Origin;
            Id = args.Id;
            Choice = args.Choice;
        }
    }
}
