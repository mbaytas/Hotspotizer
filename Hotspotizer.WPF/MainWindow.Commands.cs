//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.Commands.cs
//Version: 20151202

using System.Windows.Input;
using Hotspotizer.Helpers;
using System.Collections.Generic;

namespace Hotspotizer
{
  public partial class MainWindow
  {

    #region --- Properties ---

    //Command dictionaries//
    public readonly Dictionary<string, ICommand> commands_Manager = new Dictionary<string, ICommand>();
    public readonly Dictionary<string, ICommand> commands_Editor = new Dictionary<string, ICommand>();
    public readonly Dictionary<string, ICommand> commands_Visualizer = new Dictionary<string, ICommand>();

    //Manager//
    public RelayCommand ExitApplicationCommand { get; private set; }
    public RelayCommand CreateNewGestureCollectionCommand { get; private set; }
    public RelayCommand LoadGestureCollectionCommand { get; private set; }
    public RelayCommand SaveGestureCollectionCommand { get; private set; }
    public RelayCommand AddNewGestureCommand { get; private set; }

    //Editor//
    public RelayCommand SaveGestureCommand { get; private set; }
    public RelayCommand DiscardGestureCommand { get; private set; }
    public RelayCommand CloseCommandSelectorCommand { get; private set; }
    public RelayCommand CancelCommandSelectorCommand { get; private set; }

    //Visualizer//
    public RelayCommand PlayCommand { get; private set; }
    public RelayCommand CloseVisualizerCommand { get; private set; }

    #endregion

    #region --- Methods ---

    private void RegisterCommands()
    {
      //Manager//
      commands_Manager.Add(ManagerCommands.EXIT_APPLICATION, ExitApplicationCommand = new RelayCommand((p) => ExitApplication()));
      commands_Manager.Add(ManagerCommands.NEW_GESTURE_COLLECTION, CreateNewGestureCollectionCommand = new RelayCommand((p)=>CreateNewGestureCollection()));
      commands_Manager.Add(ManagerCommands.LOAD_GESTURE_COLLECTION, LoadGestureCollectionCommand = new RelayCommand((p)=>LoadGestureCollection()));
      commands_Manager.Add(ManagerCommands.SAVE_GESTURE_COLLECTION, SaveGestureCollectionCommand = new RelayCommand((p)=>SaveGestureCollection(), (p)=>CanSaveGestureCollection));
      commands_Manager.Add(ManagerCommands.ADD_NEW_GESTURE, AddNewGestureCommand = new RelayCommand((p)=>AddNewGesture(), (p)=>CanAddNewGesture)); //this is also available in Editor

      //Editor//
      commands_Editor.Add(EditorCommands.ADD_NEW_GESTURE, AddNewGestureCommand); //this is also available in Manager
      commands_Editor.Add(EditorCommands.CLOSE_EDITOR, DiscardGestureCommand = new RelayCommand((p) => DiscardGesture()));
      commands_Editor.Add(EditorCommands.SAVE_GESTURE, SaveGestureCommand = new RelayCommand((p)=>SaveGesture(), (p)=>CanSaveGesture));
      commands_Editor.Add(EditorCommands.MOVE_FRAME_BACKWARDS, new RelayCommand((p) => MoveCurrentFrameBackwards()));
      commands_Editor.Add(EditorCommands.MOVE_FRAME_FORWARD, new RelayCommand((p) => MoveCurrentFrameForward()));
      commands_Editor.Add(EditorCommands.CLOSE_COMMAND_SELECTOR, CloseCommandSelectorCommand = new RelayCommand((p) => CloseCommandSelector()));
      commands_Editor.Add(EditorCommands.CANCEL_COMMAND_SELECTOR, CancelCommandSelectorCommand = new RelayCommand((p) => CancelCommandSelector()));

      //Visualizer//
      commands_Visualizer.Add(VisualizerCommands.PLAY, PlayCommand = new RelayCommand((p)=>ShowVisualizer(), (p)=>CanPlay));
      commands_Visualizer.Add(VisualizerCommands.CLOSE_VISUALIZER, CloseVisualizerCommand = new RelayCommand((p)=>CloseVisualizer()));
    }

    #endregion

  }
}
