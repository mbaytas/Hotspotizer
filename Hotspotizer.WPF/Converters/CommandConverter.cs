//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: CommandConverter.cs
//Version: 20150817

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Hotspotizer.Converters
{

  [ValueConversion(typeof(ObservableCollection<Key>), typeof(string))]
  public class CommandConverter : IValueConverter
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
      ObservableCollection<Key> c = (ObservableCollection<Key>)value;

      if (c == null || c.Count == 0)
        return "";

      return string.Join(" + ", c);
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>object</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      KeyConverter kc = new KeyConverter();
      ObservableCollection<Key> c = new ObservableCollection<Key>();
      String v = (String)value;
      string[] ss = v.Split('+');

      if (ss.Length == 0)
        return c;

      foreach (string s in ss)
      {
        s.Trim();
        c.Add((Key)kc.ConvertFromString(s));
      }

      return c;
    }

    #endregion

  }

}
