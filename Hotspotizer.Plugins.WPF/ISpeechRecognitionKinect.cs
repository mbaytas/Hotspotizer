//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechRecognitionKinect.cs
//Version: 20150906

using System;
using System.IO;

namespace Hotspotizer.Plugins.WPF
{
  public interface ISpeechRecognitionKinect : ISpeechRecognition
  {
    #region --- Methods ---

    void SetInputToKinectSensor();

    #endregion
  }

}
