//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins.Speech / SpeechRecognition.cs
//Version: 20150824

//see: http://kin-educate.blogspot.gr/2012/06/speech-recognition-for-kinect-easy-way.html

using System;
using System.Linq;
using System.Speech.Recognition;

namespace HotSpotizer.Plugins.Speech.WPF
{
  class SpeechRecognition_Kinect : SpeechRecognition
  {

    protected override SpeechRecognitionEngine CreateSpeechRecognizer()
    {
      return new SpeechRecognitionEngine(GetKinectRecognizer()); //use Kinect-based recognition engine
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
