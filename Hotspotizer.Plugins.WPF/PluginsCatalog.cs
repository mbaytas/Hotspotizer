//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / PluginsCatalog.cs
//Version: 20151117

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;


namespace Hotspotizer.Plugins
{

  public static class PluginsCatalog
  {

    #region --- Fields ---

    public static CompositionContainer mefContainer;

    #endregion

    #region --- Methods ---

    public static void Init(object attributedPart)
    {
      if (mefContainer != null) return;

      AggregateCatalog partsCatalog = new AggregateCatalog();

      //TODO: replace the following code to load plugins from a subfolder, remove specific plugin references from application project and set plugin projects to copy their DLL to a "Plugins" subfolder under the folder where the executable of the app is built
      string[] assemblies = new string[]
      {
        "Hotspotizer.Plugins.SpeechSynthesis.dll",
        "Hotspotizer.Plugins.SpeechRecognition.dll"
      };

      foreach (string s in assemblies)
        partsCatalog.Catalogs.Add(new AssemblyCatalog(s));

      mefContainer = new CompositionContainer(partsCatalog);
      mefContainer.SatisfyImportsOnce(attributedPart);
      //CompositionInitializer.SatisfyImports(attributedPart);
    }

    #endregion
  }

}
