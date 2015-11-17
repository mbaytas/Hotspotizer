//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.PluginUtils.cs
//Version: 20151117

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace Hotspotizer
{
  public partial class MainWindow : Window
  {

    #region --- Fields ---

    protected static CompositionContainer mefContainer;

    #endregion

    #region --- Methods ---

    private void InitPluginsCatalog()
    {
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
      mefContainer.SatisfyImportsOnce(this);
      //CompositionInitializer.SatisfyImports(this);
    }

    private void LoadPlugins()
    {
      InitPluginsCatalog();
      LoadSpeechSynthesisPlugin();
      LoadSpeechRecognitionPlugin();
    }

    #endregion Methods

  }
}
