//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: SamenessConverter.cs
//Version: 20150817

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Hotspotizer.Converters
{

  public class SamenessConverter : IMultiValueConverter
  {

    #region --- Methods ---

    /// <summary>
    /// Converts the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>object</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null || values.Count() == 0)
        return false;

      return values.All(x => x == values[0]);
    }

    /// <summary>
    /// Converts back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetTypes">The target types.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>object array</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }

}
