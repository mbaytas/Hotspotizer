//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins.Speech / SpeechRecognition.cs
//Version: 20150905

//see: http://kin-educate.blogspot.gr/2012/06/speech-recognition-for-kinect-easy-way.html

using System.Linq;
using System.ComponentModel.Composition;
using Hotspotizer.Plugins.WPF;
using Microsoft.Speech.Recognition;
using SpeechTurtle.Utils; //using borrowed files from http://SpeechTurtle.codeplex.com
using System.Globalization;
using Microsoft.Kinect;

namespace HotSpotizer.Plugins.Speech.WPF
{
  //MEF
  [Export("SpeechRecognitionKinect", typeof(ISpeechRecognition))]
  [Export("SpeechRecognitionKinect", typeof(ISpeechRecognitionKinect))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  class SpeechRecognitionKinect : SpeechRecognition, ISpeechRecognitionKinect
  {

    protected override SpeechRecognitionEngine CreateSpeechRecognitionEngine()
    {
      RecognizerInfo kinectRecognizer = KinectUtils.GetKinectRecognizer(CultureInfo.GetCultureInfoByIetfLanguageTag("en")); //use Kinect-based recognition engine
      return (kinectRecognizer!=null)? new SpeechRecognitionEngine(kinectRecognizer) : base.CreateSpeechRecognitionEngine(); //fallback to recognition using default audio source
    }

    public void SetInputToKinectSensor()
    {
      speechRecognitionEngine.SetInputToKinectSensor(KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected));
    }

  }
}
