//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechRecognition.cs
//Version: 20150824

using System;

namespace Hotspotizer.Plugins.WPF
{
  public interface ISpeechRecognition : IPlugin
  {
    #region --- Methods ---

    void LoadGrammar(string grammar);
    //void StartFreeSpeech();
    //void StopFreeSpeech();
    //void StartVoiceCommands();
    //void StopVoiceCommands();

    #endregion

    #region --- Events ---

    event EventHandler<string> Recognized;

    #endregion
  }
}
