//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: HoldConverter.cs
//Version: 20150731

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApplication.Converters
{

  [ValueConversion(typeof(Key), typeof(string))]
  public class HoldConverter : IValueConverter
  {

    #region --- Methods ---

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((bool)value)
        return "\uE1CD";
      else
        return "\uE1CC";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }

}
