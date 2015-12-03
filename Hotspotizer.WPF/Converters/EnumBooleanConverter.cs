//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: EnumBooleanConverter.cs
//Version: 20150817

using System;
using System.Windows;
using System.Windows.Data;

namespace Hotspotizer.Converters
{

  public class EnumBooleanConverter : IValueConverter //see http://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
  {

    #region --- Methods ---

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>object</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string parameterString = parameter as string;
      if (parameterString == null || !Enum.IsDefined(value.GetType(), value))
        return DependencyProperty.UnsetValue;

      object parameterValue = Enum.Parse(value.GetType(), parameterString);
      return parameterValue.Equals(value);
    }

    /// <summary>
    /// Converts back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>object</returns>
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
