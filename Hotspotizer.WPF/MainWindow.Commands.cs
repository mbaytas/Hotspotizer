//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.Commands.cs
//Version: 20150821

using Microsoft.Kinect;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Hotspotizer.Helpers;
using Hotspotizer.Models;
using GlblRes = Hotspotizer.Properties.Resources;

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
      CreateNewGestureCollectionCommand = new RelayCommand(ManagerCommands.NEW_GESTURE_COLLECTION, CreateNewGestureCollection);
      LoadGestureCollectionCommand = new RelayCommand(ManagerCommands.LOAD_GESTURE_COLLECTION, LoadGestureCollection);
      SaveGestureCollectionCommand = new RelayCommand(ManagerCommands.SAVE_GESTURE_COLLECTION, SaveGestureCollection, CanSaveGestureCollection);
      AddNewGestureCommand = new RelayCommand(ManagerCommands.ADD_NEW_GESTURE, AddNewGesture);
      //
      SaveGestureCommand = new RelayCommand(EditorCommands.SAVE_GESTURE, SaveGesture, CanSaveGesture);
      DiscardGestureCommand = new RelayCommand(EditorCommands.CLOSE_EDITOR, DiscardGesture);
      //
      PlayCommand = new RelayCommand(VisualizerCommands.PLAY, Play, CanPlay);
      CloseVisualizerCommand = new RelayCommand(VisualizerCommands.CLOSE_VISUALIZER, CloseVisualizer);
    }

    public void CreateNewGestureCollection(object parameter)
    {
      if (MessageBox.Show(GlblRes.DiscardGestureCollectionConfirmation,
                          GlblRes.CreateNewGestureCollection, MessageBoxButton.YesNo)
          == MessageBoxResult.Yes)
        while (GestureCollection.Count > 0)
          GestureCollection.RemoveAt(0);
    }

    public void SaveGestureCollection(object parameter)
    {
      SaveFileDialog saveDialog = new SaveFileDialog() { Filter = GlblRes.HotspotizerFileFilter };

      // Write if dialog returns OK
      if (saveDialog.ShowDialog() == true)
        File.WriteAllText(saveDialog.FileName, JsonConvert.SerializeObject(GestureCollection));
    }

    public bool CanSaveGestureCollection(object parameter)
    {
      return (GestureCollection.Count > 0);
    }

    public void LoadGestureCollection(object parameter)
    {
      OpenFileDialog openDialog = new OpenFileDialog() { Filter = GlblRes.HotspotizerFileFilter };

      // Read and load if dialog returns OK
      if (openDialog.ShowDialog() == true)
      {
        string json = File.ReadAllText(openDialog.FileName);
        // DeserializeObject() does not appear to correctly deserialize Gesture objects
        // Below is a kinda-dirty solution around that
        List<Gesture> sourceList = JsonConvert.DeserializeObject<List<Gesture>>(json);
        while (GestureCollection.Count > 0)
          GestureCollection.RemoveAt(0);

        foreach (Gesture sourceGesture in sourceList)
        {
          Gesture targetGesture = new Gesture()
          {
            Name = sourceGesture.Name,
            Command = new ObservableCollection<Key>(sourceGesture.Command),
            Hold = sourceGesture.Hold,
            Joint = sourceGesture.Joint
          };

          while (targetGesture.Frames.Count > 0)
            targetGesture.Frames.RemoveAt(0);

          foreach (GestureFrame sourceFrame in sourceGesture.Frames)
          {
            GestureFrame targetFrame = new GestureFrame();
            for (int i = 0; i < 400; i++)
            {
              targetFrame.FrontCells[i] = sourceFrame.FrontCells[i];
              targetFrame.SideCells[i] = sourceFrame.SideCells[i];
            }
            targetGesture.Frames.Add(targetFrame);
          }
          GestureCollection.Add(targetGesture);
        }
      }
    }

    public void AddNewGesture(object parameter)
    {
      // Clear the initial state store
      ExGesture = null;

      // Make the new gesture and name it properly
      Gesture g = MakeNewGesture();
      g.Name = GlblRes.NewGesture;
      while (GestureCollection.Where(x => x.Name.StartsWith(g.Name)).Count() > 0)
        g.Name += " " + Convert.ToString(GestureCollection.Where(x => x.Name.StartsWith(g.Name)).Count() + 1);

      // Add the new gesture to the Gesture Collection
      GestureCollection.Add(g);

      // Go go go
      TheWorkspace.DataContext = g;
      LaunchEditor();
    }

    public void SaveGesture(object parameter)
    {
      ExGesture = null;
      KillEditor();
    }

    public bool CanSaveGesture(object parameter)
    {
      // TODO
      if (TheWorkspace == null)
        return false;

      Gesture g = (Gesture)TheWorkspace.DataContext;
      return (g != null) &&
             !String.IsNullOrEmpty(g.Name) &&
             (g.Command != null) && (g.Command.Count != 0) && !g.Command.Contains(Key.None) &&
             (g.Joint != JointType.HipCenter) &&
             !g.Frames.Any(f => f.SideCells.Count(c => c.IsHotspot) == 0);
    }

    public void DiscardGesture(object parameter)
    {
      Gesture g = (Gesture)TheWorkspace.DataContext;
      if (ExGesture == null)                                         // If the gesture is a new gesture...
        GestureCollection.Remove(g);                                 //...remove it from the Gesture Collection
      else                                                           // else if the gesture is an existing gesture being edited...
        GestureCollection[GestureCollection.IndexOf(g)] = ExGesture; //...restore it to its initial state

      KillEditor(); // Go go go
    }

    #endregion

  }
}
