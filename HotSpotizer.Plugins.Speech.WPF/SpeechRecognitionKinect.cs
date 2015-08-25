//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins.Speech / SpeechRecognition.cs
//Version: 20150825

//see: http://kin-educate.blogspot.gr/2012/06/speech-recognition-for-kinect-easy-way.html

using System;
using System.Linq;
using System.Speech.Recognition;
using System.ComponentModel.Composition;
using Hotspotizer.Plugins.WPF;

namespace HotSpotizer.Plugins.Speech.WPF
{
  //MEF
  [Export("SpeechRecognitionKinect", typeof(ISpeechRecognition))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  class SpeechRecognitionKinect : SpeechRecognition
  {

    protected override SpeechRecognitionEngine CreateSpeechRecognizer()
    {
      RecognizerInfo kinectRecognizer = GetKinectRecognizer(); //use Kinect-based recognition engine
      return (kinectRecognizer!=null)? new SpeechRecognitionEngine(kinectRecognizer) : null;
    }

    private static RecognizerInfo GetKinectRecognizer() //TODO: fix this to not hardcode en-US (see MS Kinect sample)
    {
      Func<RecognizerInfo, bool> matchingFunc = r =>
      {
        string value;
        r.AdditionalInfo.TryGetValue("Kinect", out value);
        return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
      };
      return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
    }

  }
}
