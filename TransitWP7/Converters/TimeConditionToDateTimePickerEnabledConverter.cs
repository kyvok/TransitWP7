using System.Globalization;
using System.Windows.Data;

namespace TransitWP7.Converters
{
    public class TimeConditionToDateTimePickerEnabledConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var timeCondition = (TimeCondition)value;

            switch (timeCondition)
            {
                case TimeCondition.Now:
                    return false;
                case TimeCondition.DepartingAt:
                case TimeCondition.ArrivingAt:
                    return true;
                case TimeCondition.LastArrivalTime:
                    return ((string)parameter) == "datePicker";
                default:
                    return true;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}