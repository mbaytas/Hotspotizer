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
using WpfApplication.Helpers;
using WpfApplication.Models;

namespace WpfApplication
{
  public partial class MainWindow
  {

    #region --- Properties ---

    public ICommand CreateNewGestureCollectionCommand { get; set; }
    public ICommand SaveGestureCollectionCommand { get; set; }
    public ICommand LoadGestureCollectionCommand { get; set; }
    public ICommand AddNewGestureCommand { get; set; }
    public ICommand SaveGestureCommand { get; set; }
    public ICommand DiscardGestureCommand { get; set; }
    public ICommand PlayCommand { get; set; }
    public ICommand CloseVisualizerCommand { get; set; }

    #endregion

    #region --- Methods ---

    private void registerCommands()
    {
      CreateNewGestureCollectionCommand = new RelayCommand(CreateNewGestureCollection);
      SaveGestureCollectionCommand = new RelayCommand(SaveGestureCollection, CanSaveGestureCollection);
      LoadGestureCollectionCommand = new RelayCommand(LoadGestureCollection);
      AddNewGestureCommand = new RelayCommand(AddNewGesture);
      SaveGestureCommand = new RelayCommand(SaveGesture, CanSaveGesture);
      DiscardGestureCommand = new RelayCommand(DiscardGesture);
      PlayCommand = new RelayCommand(Play, CanPlay);
      CloseVisualizerCommand = new RelayCommand(CloseVisualizer);
    }

    public void CreateNewGestureCollection(object parameter)
    {
      if (MessageBox.Show("Do you really want to discard the current gesture collection and create a new one?",
          "Create New Gesture Collection",
          MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        while (GestureCollection.Count > 0) GestureCollection.RemoveAt(0);
      }
    }

    public void SaveGestureCollection(object parameter)
    {
      // Conjure file explorer
      SaveFileDialog saveDialog = new SaveFileDialog();
      // Fucking around with file formats
      saveDialog.Filter = "Hotspotizer Gesture files (*.hsjson)|*.hsjson";
      // Write if dialog returns OK
      if (saveDialog.ShowDialog() == true)
      {
        File.WriteAllText(saveDialog.FileName, JsonConvert.SerializeObject(GestureCollection));
      }
    }
    public bool CanSaveGestureCollection(object parameter)
    {
      return (GestureCollection.Count > 0);
    }

    public void LoadGestureCollection(object parameter)
    {
      // Conjure file explorer
      OpenFileDialog openDialog = new OpenFileDialog();
      // Fucking around with file formats
      openDialog.Filter = "Hotspotizer Gesture files (*.hsjson)|*.hsjson";
      // Read and load if dialog returns OK
      if (openDialog.ShowDialog() == true)
      {
        string json = File.ReadAllText(openDialog.FileName);
        // DeserializeObject() does not appear to correctly deserialize Gesture objects
        // Below is a kinda-dirty solution around that
        List<Gesture> sourceList = JsonConvert.DeserializeObject<List<Gesture>>(json);
        while (GestureCollection.Count > 0) GestureCollection.RemoveAt(0);
        foreach (Gesture sourceGesture in sourceList)
        {
          Gesture targetGesture = new Gesture();
          targetGesture.Name = sourceGesture.Name;
          targetGesture.Command = new ObservableCollection<Key>(sourceGesture.Command);
          targetGesture.Hold = sourceGesture.Hold;
          targetGesture.Joint = sourceGesture.Joint;
          while (targetGesture.Frames.Count > 0) targetGesture.Frames.RemoveAt(0);
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
      g.Name = "New Gesture";
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
      if (TheWorkspace != null)
      {
        Gesture g = (Gesture)TheWorkspace.DataContext;
        return g != null &&
            !String.IsNullOrEmpty(g.Name) &&
            g.Command != null && g.Command.Count != 0 && !g.Command.Contains(Key.None) &&
            g.Joint != JointType.HipCenter &&
            !g.Frames.Any(f => f.SideCells.Count(c => c.IsHotspot) == 0);
      }
      else return false;
    }

    public void DiscardGesture(object parameter)
    {
      Gesture g = (Gesture)TheWorkspace.DataContext;
      // If the gesture is a new gesture, remove that motherfucker from the Gesture Collection
      if (ExGesture == null) GestureCollection.Remove(g);
      // If the gesture is an existing gesture being edited, restore the motherfucker to its initial state
      else GestureCollection[GestureCollection.IndexOf(g)] = ExGesture;
      // Go go go
      KillEditor();
    }

    #endregion

  }
}
