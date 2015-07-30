using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApplication.Converters {

  [ValueConversion(typeof(Key), typeof(string))]
  public class HoldConverter : IValueConverter {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      bool c = (bool)value;
      if (c) return "\uE1CD";
      else return "\uE1CC";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

  }

}
