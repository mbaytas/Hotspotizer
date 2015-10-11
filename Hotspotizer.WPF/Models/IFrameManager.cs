//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Models / IFrameManager.cs
//Version: 20151011

namespace Hotspotizer.Models
{
  public interface IFrameManager
  {
    #region --- Properties ---

    GestureFrame CurrentFrame { get; }

    #endregion

    #region --- Methods ---

    void AddNewFrame();
    void DeleteCurrentFrame();

    void SelectPreviousFrame(); //TODO
    void SelectNextFrame(); //TODO

    void MoveCurrentFrameBackwards();
    void MoveCurrentFrameForward();

    #endregion
  }
}