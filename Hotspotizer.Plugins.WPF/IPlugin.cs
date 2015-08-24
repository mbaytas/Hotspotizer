//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / IPlugin.cs
//Version: 20150824

using System;

namespace Hotspotizer.Plugins
{
  #region --- Methods ---

  public interface IPlugin : IDisposable
  {
    void Init();
  }

  #endregion
}
