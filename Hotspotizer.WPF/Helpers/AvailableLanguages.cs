//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: AvailableLanguages.cs
//Version: 20150823

using Hotspotizer.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;

namespace Hotspotizer.Helpers
{

  class AvailableLanguages : ObservableCollection<string>
  {
    public AvailableLanguages()
    {
      GetAvailableLanguages(this);
    }

    public static void GetAvailableLanguages(ObservableCollection<string> languages)
    {
      var cultures = GetAvailableCultures();
      languages.Clear();
      foreach (CultureInfo culture in cultures)
        languages.Add(culture.NativeName + " (" + culture.EnglishName + " [" + culture.TwoLetterISOLanguageName + "])");
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
