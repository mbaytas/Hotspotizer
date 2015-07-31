using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace WpfApplication.Models
{

  public class Gesture : INotifyPropertyChanged
  {
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
      Frames = new ObservableCollection<GestureFrame>();
    }

    #endregion

    #region --- Properties ---

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        OnPropertyChanged("Name");
      }
    }

    public ObservableCollection<Key> Command
    {
      get { return _command; }
      set
      {
        _command = value;
        OnPropertyChanged("Command");
      }
    }

    public bool Hold
    {
      get { return _hold; }
      set
      {
        _hold = value;
        OnPropertyChanged("Hold");
      }
    }

    public JointType Joint
    {
      get { return _joint; }
      set
      {
        _joint = value;
        OnPropertyChanged("Joint");
      }
    }

    public ObservableCollection<GestureFrame> Frames
    {
      get { return _frames; }
      set
      {
        _frames = value;
        OnPropertyChanged("Frames");
      }
    }

    // This really should not be in this class but whatevs
    public Boolean IsHit
    {
      get { return _isHit; }
      set
      {
        _isHit = value;
        OnPropertyChanged("IsHit");
      }
    }

    #endregion

    #region --- Events ---

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged(string propName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }

    #endregion

  }

}
