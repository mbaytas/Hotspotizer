//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins.Speech / SpeechSynthesis.cs
//Version: 20150825

using Hotspotizer.Plugins.WPF;
using System;
using System.ComponentModel.Composition;
using System.Speech.Synthesis;

namespace HotSpotizer.Plugins.Speech.WPF
{
  //MEF
  [Export("SpeechSynthesis", typeof(ISpeechSynthesis))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  class SpeechSynthesis : ISpeechSynthesis
  {
    private SpeechSynthesizer speechSynthesizer;
    
    public void Init()
    {
      speechSynthesizer = new SpeechSynthesizer();
      speechSynthesizer.SetOutputToDefaultAudioDevice();
    }

    public void Speak(string text)
    {
      speechSynthesizer.Speak(text);
    }

    #region IDisposable Support
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
  }
}
