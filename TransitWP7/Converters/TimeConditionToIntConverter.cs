using System;
using System.Globalization;
using System.Windows.Data;

namespace TransitWP7.Converters
{
    public class TimeConditionToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((TimeCondition)value)
            {
                case TimeCondition.Now:
                    return 0;
                case TimeCondition.DepartingAt:
                    return 1;
                case TimeCondition.ArrivingAt:
                    return 2;
                case TimeCondition.LastArrivalTime:
                    return 3;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return TimeCondition.Now;
                case 1:
                    return TimeCondition.DepartingAt;
                case 2:
                    return TimeCondition.ArrivingAt;
                case 3:
                    return TimeCondition.LastArrivalTime;
                default:
                    return TimeCondition.Now;
            }
        }
    }
}
