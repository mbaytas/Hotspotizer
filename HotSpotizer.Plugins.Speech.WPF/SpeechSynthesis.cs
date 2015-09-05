//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins.Speech / SpeechSynthesis.cs
//Version: 20150905

using Hotspotizer.Plugins.WPF;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis; //do not use Microsoft.Speech.Synthesis here, throws Illegal Access Error (probably needs to have the app running as administrator, or some other issue occurs when trying to use speech synthesis with it)

namespace HotSpotizer.Plugins.Speech.WPF
{
  //MEF
  [Export("SpeechSynthesis", typeof(ISpeechSynthesis))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  class SpeechSynthesis : ISpeechSynthesis
  {

    #region --- Fields ---

    private SpeechSynthesizer speechSynthesizer;

    #endregion

    #region --- Initialization ---

    public void Init()
    {
      speechSynthesizer = new SpeechSynthesizer();
      speechSynthesizer.SetOutputToDefaultAudioDevice();

      InstalledVoice voice = speechSynthesizer.GetInstalledVoices(Culture).FirstOrDefault();
      if (voice!=null)
        speechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
    }

    #endregion

    #region --- Cleanup ---

    //IDisposable//

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~SpeechSynthesis() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }

    #endregion

    #region --- Properties ---

    public CultureInfo Culture { get; } = CultureInfo.GetCultureInfoByIetfLanguageTag("en-US"); //do not use "en" here, have to use "en-US", GetInstalledVoices method doesn't support a fallback mechanism to more generic cultures

    #endregion

    #region --- Methods ---

    public void Speak(string text)
    {
      speechSynthesizer.Speak(text);
    }

    #endregion
  }
}
