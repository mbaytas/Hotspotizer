//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: AvailableLanguages.cs
//Version: 20150824

using Hotspotizer.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;

namespace Hotspotizer.Helpers
{

  class AvailableLanguages : ObservableCollection<object>
  {
    public AvailableLanguages()
    {
      GetAvailableLanguages(this);
    }

    public static void GetAvailableLanguages(ObservableCollection<object> languages)
    {
      var cultures = GetAvailableCultures();
      languages.Clear();
      foreach (CultureInfo culture in cultures)
        languages.Add(new { //Anonymous Type
                            text = culture.NativeName + " (" + culture.EnglishName + " [" + culture.TwoLetterISOLanguageName + "])",
                            value = culture.TwoLetterISOLanguageName //do not pass just culture here, ComboBox can't compare agaist it if one then sets "SelectedValue" to set the current selected item
                          }); //when used with a WPF ComboBox, make sure you set DisplayMemberPath="text" and SelectedValuePath="value"
    }

    //see http://stackoverflow.com/questions/553244/programmatic-way-to-get-all-the-available-languages-in-satellite-assemblies
    public static IEnumerable<CultureInfo> GetAvailableCultures()
    {
      List<CultureInfo> result = new List<CultureInfo>();

      ResourceManager rm = new ResourceManager(typeof(Resources));
 
      CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
      foreach (CultureInfo culture in cultures)
      {
        try
        {
          if (culture.Equals(CultureInfo.InvariantCulture)) continue; //do not use "==", won't work

          ResourceSet rs = rm.GetResourceSet(culture, true, false);
          if (rs != null)
            result.Add(culture);
        }
        catch (CultureNotFoundException)
        {
          //NOP
        }
      }
      return result;
    }

  }

}
