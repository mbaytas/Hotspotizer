//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.Commands.cs
//Version: 20150915

using System.Windows.Input;
using Hotspotizer.Helpers;

namespace Hotspotizer
{
  public partial class MainWindow
  {

    #region --- Properties ---

    //Manager//
    public ICommand CreateNewGestureCollectionCommand { get; set; }
    public ICommand LoadGestureCollectionCommand { get; set; }
    public ICommand SaveGestureCollectionCommand { get; set; }
    public ICommand AddNewGestureCommand { get; set; }

    //Editor//
    public ICommand SaveGestureCommand { get; set; }
    public ICommand DiscardGestureCommand { get; set; }
    public ICommand CloseCommandSelectorCommand { get; set; }
    public ICommand CancelCommandSelectorCommand { get; set; }

    //Visualizer//
    public ICommand PlayCommand { get; set; }
    public ICommand CloseVisualizerCommand { get; set; }

    #endregion

    #region --- Methods ---

    private void RegisterCommands()
    {
      //Manager//
      CreateNewGestureCollectionCommand = new RelayCommand(ManagerCommands.NEW_GESTURE_COLLECTION, (p)=>CreateNewGestureCollection());
      LoadGestureCollectionCommand = new RelayCommand(ManagerCommands.LOAD_GESTURE_COLLECTION, (p)=>LoadGestureCollection());
      SaveGestureCollectionCommand = new RelayCommand(ManagerCommands.SAVE_GESTURE_COLLECTION, (p)=>SaveGestureCollection(), (p)=>CanSaveGestureCollection());
      AddNewGestureCommand = new RelayCommand(ManagerCommands.ADD_NEW_GESTURE, (p)=>AddNewGesture());

      //Editor//
      SaveGestureCommand = new RelayCommand(EditorCommands.SAVE_GESTURE, (p)=>SaveGesture(), (p)=>CanSaveGesture());
      DiscardGestureCommand = new RelayCommand(EditorCommands.CLOSE_EDITOR, (p)=>DiscardGesture());
      CloseCommandSelectorCommand = new RelayCommand(EditorCommands.CLOSE_COMMAND_SELECTOR, (p) => CloseCommandSelector());
      CancelCommandSelectorCommand = new RelayCommand(EditorCommands.CANCEL_COMMAND_SELECTOR, (p) => CancelCommandSelector());

      //Visualizer//
      PlayCommand = new RelayCommand(VisualizerCommands.PLAY, (p)=>ShowVisualizer(), (p)=>CanPlay());
      CloseVisualizerCommand = new RelayCommand(VisualizerCommands.CLOSE_VISUALIZER, (p)=>CloseVisualizer());
    }

    #endregion

  }
}
