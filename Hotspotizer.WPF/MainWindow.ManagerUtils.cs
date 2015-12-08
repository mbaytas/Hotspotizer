//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: MainWindow.ManagerUtils.cs
//Version: 20151208

using System.Windows;
using System.Windows.Controls;
using Hotspotizer.Models;
using GlblRes = Hotspotizer.Properties.Resources;
using Microsoft.Win32;
using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;

namespace Hotspotizer
{
  public partial class MainWindow : IManager
  {

    #region --- Properties ---

    public bool CanSaveGestureCollection
    {
      get { return (GestureCollection.Count > 0); }
    }

    public bool CanAddNewGesture
    {
      get { return !EditorVisible || CanSaveGesture; }
    }

    #endregion

    #region --- Methods ---

    #region IManager methods

    public void CreateNewGestureCollection()
    {
      if (MessageBox.Show(GlblRes.DiscardGestureCollectionConfirmation,
                          GlblRes.CreateNewGestureCollection, MessageBoxButton.YesNo)
          == MessageBoxResult.Yes)
        GestureCollection.Clear();

      if (GestureCollectionLoaded != null)
        GestureCollectionLoaded(this, EventArgs.Empty);
    }

    public void LoadGestureCollection()
    {
      OpenFileDialog openDialog = new OpenFileDialog() { Filter = GlblRes.HotspotizerFileFilter };
      if (openDialog.ShowDialog() == true) // Read and load if dialog returns OK
        LoadGestureCollection(openDialog.FileName);
    }

    public void LoadGestureCollection(string filename)
    {
      string json = File.ReadAllText(filename);
      // DeserializeObject() does not appear to correctly deserialize Gesture objects
      // Below is a kinda-dirty solution around that
      List<Gesture> sourceList = JsonConvert.DeserializeObject<List<Gesture>>(json);
      GestureCollection.Clear();

      foreach (Gesture sourceGesture in sourceList)
      {
        RemoveNoneKeys(sourceGesture.Command); //Seems somewhere at the serialization or deserialization Key.None creeps in, so remove it

        //copy sourceGesture to targetGesture //TODO: check why this copying is needed
        Gesture targetGesture = new Gesture()
        {
          Name = sourceGesture.Name,
          Command = new ObservableCollection<Key>(sourceGesture.Command),
          Hold = sourceGesture.Hold,
          Joint = sourceGesture.Joint
        };
        //copy the frames too (note: this is not the same as DeepCopyGestureFrame)
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

        if (GestureCollectionLoaded != null)
          GestureCollectionLoaded(this, EventArgs.Empty);
      }
    }

    private void RemoveNoneKeys(ObservableCollection<Key> keys)
    {
      while (keys.Remove(Key.None));
    }

    public void SaveGestureCollection()
    {
      SaveFileDialog saveDialog = new SaveFileDialog() { Filter = GlblRes.HotspotizerFileFilter };
      if (saveDialog.ShowDialog() == true) // Write if dialog returns OK
        SaveGestureCollection(saveDialog.FileName);
    }

    public void SaveGestureCollection(string filename)
    {
      File.WriteAllText(filename, JsonConvert.SerializeObject(GestureCollection));
    }

    public Gesture AddNewGesture()
    {
      if (!CanAddNewGesture)
        return null; //do not add a new gesture if any currently edited gesture doesn't contain enough data to be allowed to save

      if (EditorVisible)
        SaveGesture(); //CanAddNewGesture has already checked if CanSaveGesture when EditorVisible

      // Clear the initial state store
      ExGesture = null;

      // Make the new gesture and name it properly
      Gesture g = MakeNewGesture();
      g.Name = GlblRes.NewGesture;
      while (GestureCollection.Where(x => x.Name.StartsWith(g.Name)).Count() > 0)
        g.Name += " " + Convert.ToString(GestureCollection.Where(x => x.Name.StartsWith(g.Name)).Count() + 1);

      // Add the new gesture to the Gesture Collection
      GestureCollection.Add(g);

      // Go
      TheWorkspace.DataContext = g;
      ShowEditor();

      return g;
    }

    public void EditGesture(Gesture g)
    {
      // Store the initial state of the gesture being edited
      ExGesture = DeepCopyGesture(g);
      // Go
      TheWorkspace.DataContext = g;
      ShowEditor();
    }

    public void DeleteGesture(Gesture g)
    {
      if (MessageBox.Show(string.Format(GlblRes.DeleteGestureFromCollectionConfirmation, g.Name),
                    GlblRes.DeleteGestureFromCollection, MessageBoxButton.YesNo)
          == MessageBoxResult.Yes)
        GestureCollection.Remove(g);
    }

    #endregion

    #endregion

    #region --- Events ---

    public event EventHandler GestureCollectionLoaded;

    public void EditGestureButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;
      EditGesture(g);
    }

    public void DeleteGestureButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;
      DeleteGesture(g);
    }

    #endregion

  }
}
