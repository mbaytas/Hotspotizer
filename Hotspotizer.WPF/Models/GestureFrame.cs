//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: GestureFrame.cs
//Version: 20150817

namespace Hotspotizer.Models
{

  public class GestureFrame
  {

    #region --- Properties ---

    public GestureFrameCell[] FrontCells { get; set; }
    public GestureFrameCell[] SideCells { get; set; }

    #endregion

    #region --- Constructor ---

    public GestureFrame()
    {
      FrontCells = new GestureFrameCell[400];
      SideCells = new GestureFrameCell[400];
    }

    #endregion

  }

}
