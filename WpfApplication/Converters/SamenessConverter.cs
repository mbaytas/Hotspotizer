//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: SamenessConverter.cs
//Version: 20150731

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WpfApplication.Converters
{

  public class SamenessConverter : IMultiValueConverter
  {

    #region --- Methods ---

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null || values.Count() == 0)
        return false;

      return values.All(x => x == values[0]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

  }

}
