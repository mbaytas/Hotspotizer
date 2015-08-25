//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins.Speech / SpeechRecognition.cs
//Version: 20150825

using Hotspotizer.Plugins.WPF;
using System;
using System.ComponentModel.Composition;
using System.Speech.Recognition;

namespace HotSpotizer.Plugins.Speech.WPF
{
  //MEF
  [Export("SpeechRecognition", typeof(ISpeechRecognition))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  class SpeechRecognition : ISpeechRecognition
  {
    #region --- Fields ---

    private SpeechRecognitionEngine speechRecognizer;

    #endregion

    public event EventHandler<string> Recognized;

    public void Init()
    {
      speechRecognizer = CreateSpeechRecognizer();
      if (speechRecognizer != null)
      {
        speechRecognizer.SpeechRecognized += SpeechRecognized;
        speechRecognizer.SpeechHypothesized += SpeechHypothesized;
        speechRecognizer.SpeechRecognitionRejected += SpeechRecognitionRejected;
      }
    }

    protected virtual SpeechRecognitionEngine CreateSpeechRecognizer()
    {
      return new SpeechRecognitionEngine(); //use current system default recognition engine
    }

    private void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
      throw new NotImplementedException();
    }

    public void LoadGrammar(string grammar)
    {
      throw new NotImplementedException();
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
    // ~SpeechRecognition() {
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
