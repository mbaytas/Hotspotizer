using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApplication.Converters
{

  [ValueConversion(typeof(Key), typeof(string))]
  public class CommandConverterWithIcon : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      ObservableCollection<Key> c = (ObservableCollection<Key>)value;
      if (c == null || c.Count == 0) return "\uE144";
      return "\uE144 " + string.Join(" + ", c);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      KeyConverter kc = new KeyConverter();
      ObservableCollection<Key> c = new ObservableCollection<Key>();
      String v = (String)value;
      string[] ss = v.Replace("\uE144", "").TrimStart().Split('+');
      if (ss.Length == 0) return c;
      foreach (string s in ss)
      {
        s.Trim();
        c.Add((Key)kc.ConvertFromString(s));
      }
      return c;
    }

  }

}
