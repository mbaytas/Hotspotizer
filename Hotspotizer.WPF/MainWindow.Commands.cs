//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.Commands.cs
//Version: 20150915

using System.Windows.Input;
using Hotspotizer.Helpers;
using System.Collections.Generic;

namespace Hotspotizer
{
  public partial class MainWindow
  {

    #region --- Properties ---

    public readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

    //Manager//
    public RelayCommand CreateNewGestureCollectionCommand { get; set; }
    public RelayCommand LoadGestureCollectionCommand { get; set; }
    public RelayCommand SaveGestureCollectionCommand { get; set; }
    public RelayCommand AddNewGestureCommand { get; set; }

    //Editor//
    public RelayCommand SaveGestureCommand { get; set; }
    public RelayCommand DiscardGestureCommand { get; set; }
    public RelayCommand CloseCommandSelectorCommand { get; set; }
    public RelayCommand CancelCommandSelectorCommand { get; set; }

    //Visualizer//
    public RelayCommand PlayCommand { get; set; }
    public RelayCommand CloseVisualizerCommand { get; set; }

    #endregion

    #region --- Methods ---

    private void RegisterCommands()
    {
      //Manager//
      Commands.Add(ManagerCommands.NEW_GESTURE_COLLECTION, CreateNewGestureCollectionCommand = new RelayCommand((p)=>CreateNewGestureCollection()));
      Commands.Add(ManagerCommands.LOAD_GESTURE_COLLECTION, LoadGestureCollectionCommand = new RelayCommand((p)=>LoadGestureCollection()));
      Commands.Add(ManagerCommands.SAVE_GESTURE_COLLECTION, SaveGestureCollectionCommand = new RelayCommand((p)=>SaveGestureCollection(), (p)=>CanSaveGestureCollection()));
      Commands.Add(ManagerCommands.ADD_NEW_GESTURE, AddNewGestureCommand = new RelayCommand((p)=>AddNewGesture()));

      //Editor//
      Commands.Add(EditorCommands.SAVE_GESTURE, SaveGestureCommand = new RelayCommand((p)=>SaveGesture(), (p)=>CanSaveGesture()));
      Commands.Add(EditorCommands.CLOSE_EDITOR, DiscardGestureCommand = new RelayCommand((p)=>DiscardGesture()));
      Commands.Add(EditorCommands.CLOSE_COMMAND_SELECTOR, CloseCommandSelectorCommand = new RelayCommand((p) => CloseCommandSelector()));
      Commands.Add(EditorCommands.CANCEL_COMMAND_SELECTOR, CancelCommandSelectorCommand = new RelayCommand((p) => CancelCommandSelector()));

      //Visualizer//
      Commands.Add(VisualizerCommands.PLAY, PlayCommand = new RelayCommand((p)=>ShowVisualizer(), (p)=>CanPlay()));
      Commands.Add(VisualizerCommands.CLOSE_VISUALIZER, CloseVisualizerCommand = new RelayCommand((p)=>CloseVisualizer()));
    }

    #endregion

  }
}
