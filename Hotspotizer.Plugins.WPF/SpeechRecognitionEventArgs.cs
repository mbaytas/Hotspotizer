//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / SpeechRecognitionEventArgs.cs
//Version: 20151117

using System;

namespace Hotspotizer.Plugins
{

  public class SpeechRecognitionEventArgs : EventArgs
  {
    #region --- Fields ---

    public readonly string command;
    public readonly double confidence; //a value from 0 to 1 (1=most confident)

    #endregion

    #region --- Initialization ---

    public SpeechRecognitionEventArgs(string command, double confidence)
    {
      this.command = command;
      this.confidence = confidence;
    }

    #endregion
  }
}