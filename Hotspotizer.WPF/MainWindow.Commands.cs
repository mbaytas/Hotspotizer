//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.Commands.cs
//Version: 20150908

using System.Windows.Input;
using Hotspotizer.Helpers;

namespace Hotspotizer
{
  public partial class MainWindow
  {

    #region --- Properties ---

    public ICommand CreateNewGestureCollectionCommand { get; set; }
    public ICommand LoadGestureCollectionCommand { get; set; }
    public ICommand SaveGestureCollectionCommand { get; set; }
    public ICommand AddNewGestureCommand { get; set; }

    public ICommand SaveGestureCommand { get; set; }
    public ICommand DiscardGestureCommand { get; set; }

    public ICommand PlayCommand { get; set; }
    public ICommand CloseVisualizerCommand { get; set; }

    #endregion

    #region --- Methods ---

    private void RegisterCommands()
    {
      CreateNewGestureCollectionCommand = new RelayCommand(ManagerCommands.NEW_GESTURE_COLLECTION, (p)=>CreateNewGestureCollection());
      LoadGestureCollectionCommand = new RelayCommand(ManagerCommands.LOAD_GESTURE_COLLECTION, (p)=>LoadGestureCollection());
      SaveGestureCollectionCommand = new RelayCommand(ManagerCommands.SAVE_GESTURE_COLLECTION, (p)=>SaveGestureCollection(), (p)=>CanSaveGestureCollection());
      AddNewGestureCommand = new RelayCommand(ManagerCommands.ADD_NEW_GESTURE, (p)=>AddNewGesture());
      //
      SaveGestureCommand = new RelayCommand(EditorCommands.SAVE_GESTURE, (p)=>SaveGesture(), (p)=>CanSaveGesture());
      DiscardGestureCommand = new RelayCommand(EditorCommands.CLOSE_EDITOR, (p)=>DiscardGesture());
      //
      PlayCommand = new RelayCommand(VisualizerCommands.PLAY, (p)=>ShowVisualizer(), (p)=>CanPlay());
      CloseVisualizerCommand = new RelayCommand(VisualizerCommands.CLOSE_VISUALIZER, (p)=>CloseVisualizer());
    }

    #endregion

  }
}
