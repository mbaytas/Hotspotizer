using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApplication.Converters {

  public class EnumBooleanConverter : IValueConverter {
    // stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      string parameterString = parameter as string;
      if (parameterString == null) return DependencyProperty.UnsetValue;
      if (Enum.IsDefined(value.GetType(), value) == false) return DependencyProperty.UnsetValue;
      object parameterValue = Enum.Parse(value.GetType(), parameterString);
      return parameterValue.Equals(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      string parameterString = parameter as string;
      if (parameterString == null) return DependencyProperty.UnsetValue;
      return Enum.Parse(targetType, parameterString);
    }

  }

}
