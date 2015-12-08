//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: Models / GestureFrameCell.cs
//Version: 20150908

using Newtonsoft.Json;
using System.ComponentModel;

namespace Hotspotizer.Models
{

  public class GestureFrameCell : INotifyPropertyChanged
  {

    #region --- Constants ---

    public const string PROPERTY_IS_HOTSPOT = "IsHotspot";

    #endregion

    #region --- Fields ---

    public int IndexInFrame;

    [JsonIgnore]
    public int TopCM { get { return 150 - 15 * (int)(IndexInFrame / 20); } }
    [JsonIgnore]
    public int BottomCM { get { return 135 - 15 * (int)(IndexInFrame / 20); } }
    [JsonIgnore]
    public int RightCM { get { return -135 + 15 * (int)(IndexInFrame % 20); } }
    [JsonIgnore]
    public int LeftCM { get { return -150 + 15 * (int)(IndexInFrame % 20); } }

    private bool _isHotspot = false;

    #endregion

    #region --- Properties ---

    public bool IsHotspot
    {
      get { return _isHotspot; }
      set
      {
        _isHotspot = value;
        OnPropertyChanged(PROPERTY_IS_HOTSPOT);
      }
    }

    #endregion

    #region --- Events ---

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged(string propName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }

    #endregion

  }

}
