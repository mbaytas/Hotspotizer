//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / ISpeechRecognitionKinect.cs
//Version: 20151117

using System;
using System.IO;

namespace Hotspotizer.Plugins
{
  public interface ISpeechRecognitionKinect : ISpeechRecognition
  {
    #region --- Methods ---

    void SetInputToKinectSensor();

    #endregion
  }

}
