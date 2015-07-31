//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: EnumBooleanConverter.cs
//Version: 20150731

using System;
using System.Windows;
using System.Windows.Data;

namespace WpfApplication.Converters
{

  public class EnumBooleanConverter : IValueConverter //see http://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
  {

    #region --- Methods ---

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string parameterString = parameter as string;
      if (parameterString == null || !Enum.IsDefined(value.GetType(), value))
        return DependencyProperty.UnsetValue;

      object parameterValue = Enum.Parse(value.GetType(), parameterString);
      return parameterValue.Equals(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string parameterString = parameter as string;
      if (parameterString == null)
        return DependencyProperty.UnsetValue;

      return Enum.Parse(targetType, parameterString);
    }

    #endregion

  }

}
