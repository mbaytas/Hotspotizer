//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.PluginUtils.cs
//Version: 20151117

using System.Windows;
using Hotspotizer.Plugins;

namespace Hotspotizer
{

  public partial class MainWindow : Window
  {
    #region --- Methods ---

    private void LoadPlugins()
    {
      PluginsCatalog.Init(this);
      LoadSpeechSynthesisPlugin();
      LoadSpeechRecognitionPlugin();
    }

    #endregion

  }
}
