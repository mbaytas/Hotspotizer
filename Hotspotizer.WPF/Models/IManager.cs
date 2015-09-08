//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: Models / IManager.cs
//Version: 20150908

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hotspotizer.Models
{
  public interface IManager
  {
    ObservableCollection<Gesture> GestureCollection { get; set; }
    ICommand SaveGestureCollectionCommand { get; set; }

    void AddNewGesture();
    bool CanSaveGestureCollection();
    void CreateNewGestureCollection();
    void LoadGestureCollection();
    void SaveGestureCollection();
  }
}