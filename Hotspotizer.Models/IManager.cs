//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: Models / IManager.cs
//Version: 20151208

using System.Collections.ObjectModel;

namespace Hotspotizer.Models
{
  public interface IManager
  {

    #region --- Properties ---

    ObservableCollection<Gesture> GestureCollection { get; set; }

    bool CanSaveGestureCollection { get; }
    bool CanAddNewGesture { get; }

    #endregion

    #region --- Methods ---

    Gesture AddNewGesture();
    void CreateNewGestureCollection();
    void LoadGestureCollection();
    void SaveGestureCollection();
    void EditGesture(Gesture g);
    void DeleteGesture(Gesture g);

    #endregion
  }
}