//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Hotspotizer.Plugins / IPlugin.cs
//Version: 20151117

using System;

namespace Hotspotizer.Plugins
{

  public interface IPlugin : IDisposable
  {
    #region --- Methods ---

    void Init();

    #endregion
  }

}
