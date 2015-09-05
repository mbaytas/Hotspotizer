//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechSynthesis.cs
//Version: 20150905

using System.Globalization;

namespace Hotspotizer.Plugins.WPF
{
  public interface ISpeechSynthesis : IPlugin
  {

    #region --- Properties ---

    CultureInfo Culture { get; }

    #endregion

    #region --- Methods ---

    void Speak(string text);

    #endregion
  }
}
