﻿//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: HoldConverter.cs
//Version: 20150817

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Hotspotizer.Converters
{

  [ValueConversion(typeof(Key), typeof(string))]
  public class HoldConverter : IValueConverter
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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((bool)value)? "\uE1CD" : "\uE1CC";
    }

    /// <summary>
    /// Converts back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>object</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }

}
