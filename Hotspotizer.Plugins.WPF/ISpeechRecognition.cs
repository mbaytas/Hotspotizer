//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechRecognition.cs
//Version: 20150906

using System;
using System.IO;

namespace Hotspotizer.Plugins.WPF
{
  public interface ISpeechRecognition : IPlugin
  {
    #region --- Properties ---

    bool AcousticModelAdaptation { get; set; }

    #endregion

    #region --- Methods ---

    void LoadGrammar(string grammar, string name);
    void LoadGrammar(Stream stream, string name);
    void SetInputToDefaultAudioDevice();
    void Start();

    #endregion

    #region --- Events ---

    event EventHandler<SpeechRecognitionEventArgs> Recognized;
    event EventHandler NotRecognized;

    #endregion
  }

}
