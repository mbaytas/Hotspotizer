//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechRecognition.cs
//Version: 20150905

using System;

namespace Hotspotizer.Plugins.WPF
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