//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Models / Gesture.cs
//Version: 20150908

using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Hotspotizer.Models
{

  public class Gesture : INotifyPropertyChanged
  {

    #region --- Constants ---

    public const string PROPERTY_NAME = "Name";
    public const string PROPERTY_COMMAND = "Command";
    public const string PROPERTY_HOLD = "Hold";
    public const string PROPERTY_JOINT = "Joint";
    public const string PROPERTY_FRAMES = "Frames";
    public const string PROPERTY_IS_HIT = "IsHit";

    #endregion

    #region --- Fields ---

    private string _name;
    private ObservableCollection<Key> _command;
    private bool _hold;
    private JointType _joint;
    private ObservableCollection<GestureFrame> _frames;

    [JsonIgnore] //TODO: is JsonIgnore even needed at a private field?
    private Boolean _isHit = false; //TODO: should the JsonIgnore be at the respective property instead?

    #endregion

    #region --- Initialization ---

    public Gesture()
    {
      Frames = new ObservableCollection<GestureFrame>(); //TODO: why just initialize Frames to empty collection and to this for Command property too? Maybe other code checks it for null somewhere?
    }

    #endregion

    #region --- Properties ---

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        OnPropertyChanged(PROPERTY_NAME);
      }
    }

    public ObservableCollection<Key> Command
    {
      get { return _command; }
      set
      {
        _command = value;
        OnPropertyChanged(PROPERTY_COMMAND);
      }
    }

    public bool Hold
    {
      get { return _hold; }
      set
      {
        _hold = value;
        OnPropertyChanged(PROPERTY_HOLD);
      }
    }

    public JointType Joint
    {
      get { return _joint; }
      set
      {
        _joint = value;
        OnPropertyChanged(PROPERTY_JOINT);
      }
    }

    public ObservableCollection<GestureFrame> Frames
    {
      get { return _frames; }
      set
      {
        _frames = value;
        OnPropertyChanged(PROPERTY_FRAMES);
      }
    }

    // This really should not be in this class but whatevs
    public Boolean IsHit
    {
      get { return _isHit; }
      set
      {
        _isHit = value;
        OnPropertyChanged(PROPERTY_IS_HIT);
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
