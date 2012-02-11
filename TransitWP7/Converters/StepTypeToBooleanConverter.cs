namespace TransitWP7.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class StepTypeToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Transforms a StepType into a boolean value
        /// </summary>
        /// <param name="value">The value to convert from</param>
        /// <param name="targetType">Type targeted</param>
        /// <param name="parameter">Optional parameters</param>
        /// <param name="culture">Culture to use</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stepType = value as ItineraryStep.ItineraryStepType?;
            var stepTypeMatch = Enum.Parse(typeof(ItineraryStep.ItineraryStepType), parameter.ToString(), true) as ItineraryStep.ItineraryStepType?;
            return stepType == stepTypeMatch;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
