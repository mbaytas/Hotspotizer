using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace WpfApplication.Models {

  public class Gesture : INotifyPropertyChanged {

    private string _name;
    public string Name {
      get { return _name; }
      set {
        _name = value;
        OnPropertyChanged("Name");
      }
    }

    private ObservableCollection<Key> _command;
    public ObservableCollection<Key> Command {
      get { return _command; }
      set {
        _command = value;
        OnPropertyChanged("Command");
      }
    }

    private bool _hold;
    public bool Hold {
      get { return _hold; }
      set {
        _hold = value;
        OnPropertyChanged("Hold");
      }
    }

    private JointType _joint;
    public JointType Joint {
      get { return _joint; }
      set {
        _joint = value;
        OnPropertyChanged("Joint");
      }
    }

    private ObservableCollection<GestureFrame> _frames;
    public ObservableCollection<GestureFrame> Frames {
      get { return _frames; }
      set {
        _frames = value;
        OnPropertyChanged("Frames");
      }
    }

    // This really should not be in this class but whatevs
    [JsonIgnore]
    private Boolean _isHit = false;
    public Boolean IsHit {
      get { return _isHit; }
      set {
        _isHit = value;
        OnPropertyChanged("IsHit");
      }
    }

    public Gesture() {
      Frames = new ObservableCollection<GestureFrame>();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string propName) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }

  }

  public class GestureFrame {

    public GestureFrameCell[] FrontCells { get; set; }
    public GestureFrameCell[] SideCells { get; set; }

    public GestureFrame() {
      FrontCells = new GestureFrameCell[400];
      SideCells = new GestureFrameCell[400];
    }
  }

  public class GestureFrameCell : INotifyPropertyChanged {
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
    public bool IsHotspot {
      get { return _isHotspot; }
      set {
        _isHotspot = value;
        OnPropertyChanged("IsHotspot");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string propName) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }
  }

}
