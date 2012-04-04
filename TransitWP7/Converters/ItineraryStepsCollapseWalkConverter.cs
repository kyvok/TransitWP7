namespace TransitWP7.Converters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Data;
    using TransitWP7.Model;

    public class ItineraryStepsCollapseWalkConverter : IValueConverter
    {
        /// <summary>
        /// Collapses the walking steps so that only one icon is shown
        /// </summary>
        /// <param name="value">The value to convert from</param>
        /// <param name="targetType">Type targeted</param>
        /// <param name="parameter">Optional parameters</param>
        /// <param name="culture">Culture to use</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itinerarySteps = (ObservableCollection<ItineraryStep>)value;

            var collapsedSteps = new ObservableCollection<CollapsedItineraryStep>();
            foreach (var itineraryStep in itinerarySteps)
            {
                var step = new CollapsedItineraryStep(itineraryStep.IconType, itineraryStep.BusNumber);
                if (collapsedSteps.Count != 0
                    && collapsedSteps[collapsedSteps.Count - 1].IconType[0] == 'W'
                    && step.IconType[0] == 'W')
                {
                    continue;
                }

                collapsedSteps.Add(step);
            }

            return collapsedSteps;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public class CollapsedItineraryStep
        {
            public CollapsedItineraryStep(string iconType, string busNumber)
            {
                this.IconType = iconType;
                this.BusNumber = busNumber;
            }

            public string IconType { get; set; }

            public string BusNumber { get; set; }
        }
    }
}
