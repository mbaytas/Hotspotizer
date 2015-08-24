//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechSynthesis.cs
//Version: 20150824

namespace Hotspotizer.Plugins.WPF
{
  public interface ISpeechSynthesis : IPlugin
  {
    #region --- Methods ---

    void Speak(string text);

    #endregion
  }
}
