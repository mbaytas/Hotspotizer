//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.PluginUtils.cs
//Version: 20150905

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
      partsCatalog.Catalogs.Add(new AssemblyCatalog("Hotspotizer.Plugins.Speech.dll"));
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
