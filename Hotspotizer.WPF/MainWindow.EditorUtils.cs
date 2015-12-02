//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.EditorUtils.cs
//Version: 20151203

#define ALLOW_SAVING_GESTURE_WITH_NO_COMMAND_OR_JOINT_AND_HOTSPOTS

using HelixToolkit.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Hotspotizer.Helpers;
using Hotspotizer.Models;
using Microsoft.Kinect;

namespace Hotspotizer
{
  public partial class MainWindow : IFrameManager
  {

    #region --- Fields ---

    // Stores the initial state of the gesture being edited
    Gesture ExGesture = null;

    // Stores the initial state of the Gesture.Command being edited
    ObservableCollection<Key> CommandBackup = new ObservableCollection<Key>();
    // Stores the keys being pressed while editing a Gesture.Command
    List<Key> KeysBeingPressed = new List<Key>();

    #endregion

    #region --- Initialization ---

    /// <summary>
    /// Show the Editor UI
    /// </summary>
    public void ShowEditor()
    {
      // Collision stuff for highlights during testing
      CollisionTimes = new List<List<DateTime>>();
      CollisionStates = new List<JointCollisionStates[]>();
      CollisionHighlights_3D = new Model3DGroup();
      foreach (Gesture gg in GestureCollection)
      {
        CollisionTimes.Add(new List<DateTime>());
        foreach (GestureFrame f in gg.Frames) CollisionTimes.Last().Add(new DateTime());
        CollisionStates.Add(new JointCollisionStates[2] { JointCollisionStates.OutThere, JointCollisionStates.OutThere });
        CollisionHighlights_3D.Children.Add(new Model3DGroup());
      }
      HotspotCellsModelVisual3D_Hit_Editor.Content = CollisionHighlights_3D;

      // Initialize variable to store the initial state of the Gesture.Command being set
      CommandBackup = new ObservableCollection<Key>();

      EnableKinect_Editor();

      EditorVisible = true;

      FramesListBox.SelectedIndex = 0; // Reset Frame selection

      // Kill keyboard control on 3D grid
      EventLogic.RemoveRoutedEventHandlers(ViewPort3D_Editor.CameraController, HelixToolkit.Wpf.CameraController.KeyDownEvent);
    }

    #endregion

    #region --- Cleanup ---

    /// <summary>
    /// Close the Editor UI and go back to the Manager
    /// </summary>
    public void CloseEditor()
    {
      DisableKinect_Editor();
      TheWorkspace.DataContext = InitGesture;        // Clean up DataContext
      EditorVisible = false;
    }

    /// <summary>
    /// Close command selector UI
    /// </summary>
    public void CloseCommandSelector()
    {
      RemoveHandler(Keyboard.PreviewKeyDownEvent, (KeyEventHandler)SetCommand_KeyDown);
      RemoveHandler(Keyboard.PreviewKeyUpEvent, (KeyEventHandler)SetCommand_KeyUp);
      EditorSetCommandOverlay.Visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Cancel command selection
    /// </summary>
    public void CancelCommandSelector()
    {
      CloseCommandSelector();
      // De-update gesture
      Gesture g = (Gesture)TheWorkspace.DataContext;
      g.Command = new ObservableCollection<Key>(CommandBackup);
    }

    #endregion

    #region --- Properties ---

    public bool EditorVisible
    {
      get { return (TheEditor != null) && (TheEditor.Visibility == Visibility.Visible) && ((EditorOverlay == null) || EditorOverlay.Visibility == Visibility.Hidden); }
      private set
      {
        if (value)
        {
          TheEditor.Visibility = Visibility.Visible;
          EditorOverlay.Visibility = Visibility.Hidden;
          ManagerOverlay.Visibility = Visibility.Visible; // Hide Manager
          AddNewGestureManagerOverlayButton.Visibility = Visibility.Visible; //make sure this is visible, since it is hidden at Visualizer
        }
        else
        {
          EditorOverlay.Visibility = Visibility.Visible; // Hide Editor
          ManagerOverlay.Visibility = Visibility.Hidden; // Show Manager
        }
      }
    }

    public GestureFrame CurrentFrame //IFrameManager property
    {
      get { return (GestureFrame)FramesListBox.SelectedItem; }
    }

    public bool CanSaveGesture
    {
      get
      {
        //TODO (???)
        if (TheWorkspace == null) return false;
        Gesture g = (Gesture)TheWorkspace.DataContext;
        return (g != null) &&
               !String.IsNullOrEmpty(g.Name)
               && (HasSelectedCommand
               #if ALLOW_SAVING_GESTURE_WITH_NO_COMMAND_OR_JOINT_AND_HOTSPOTS
               ||
               #else
               &&
               #endif
               HasSelectedJointAndHotspotsForEachFrame);
      }
    }

    public bool HasSelectedCommand
    {
      get
      {
        if (TheWorkspace == null) return false;
        Gesture g = (Gesture)TheWorkspace.DataContext;
        return (g.Command != null) && (g.Command.Count != 0) && !g.Command.Contains(Key.None);
      }
    }

    public bool HasSelectedJointAndHotspotsForEachFrame
    {
      get { return HasSelectedJoint && HasHotspotsForEachFrame; }
    }

    public bool HasSelectedJoint
    {
      get
      {
        if (TheWorkspace == null) return false;
        Gesture g = (Gesture)TheWorkspace.DataContext;
        return (g.Joint != JointType.HipCenter);
      }
    }

    public bool HasHotspotsForEachFrame
    {
      get
      {
        if (TheWorkspace == null) return false;
        Gesture g = (Gesture)TheWorkspace.DataContext;
        return !g.Frames.Any(f => f.SideCells.Count(c => c.IsHotspot) == 0);
      }
    }

    #endregion

    #region --- Methods ---

    public bool SaveGesture()
    {
      if (!CanSaveGesture) return false;

      ExGesture = null;
      CloseEditor();
      return true;
    }

    /// <summary>
    /// Undo changes done to gesture (or if it was a new one discard it)
    /// </summary>
    public void DiscardGesture()
    {
      Gesture g = (Gesture)TheWorkspace.DataContext;

      if (ExGesture == null)                                         // If the gesture is a new gesture...
        GestureCollection.Remove(g);                                 //...remove it from the Gesture Collection
      else                                                           // else if the gesture is an existing gesture being edited...
        GestureCollection[GestureCollection.IndexOf(g)] = ExGesture; //...restore it to its initial state

      CloseEditor();
    }

    #region IFrameManager methods

    public void AddNewFrame()
    {
      Gesture g = (Gesture)TheWorkspace.DataContext;
      if (g == null) return;

      g.Frames.Add(MakeNewGestureFrame());                              // Add the frame to the Gesture object
      CollisionTimes[GestureCollection.IndexOf(g)].Add(new DateTime()); // Extend CollisionTimes to keep track of collisions
      FramesListBox.SelectedIndex = FramesListBox.Items.Count - 1;      // Select the newly added frame on the timeline

      SyncEditorGrids();
    }

    public void SelectPreviousFrame()
    {
      int selectedIndex = FramesListBox.SelectedIndex;
      FramesListBox.SelectedIndex = (selectedIndex != 0) ? selectedIndex - 1 : FramesListBox.Items.Count - 1;
    }

    public void SelectNextFrame()
    {
      int selectedIndex = FramesListBox.SelectedIndex;
      FramesListBox.SelectedIndex = (selectedIndex != FramesListBox.Items.Count-1) ? selectedIndex + 1 : 0;
    }

    public void DeleteCurrentFrame()
    {
      DeleteFrame(CurrentFrame);
    }

    public void MoveCurrentFrameForward()
    {
      MoveFrameForward(CurrentFrame);
    }

    public void MoveCurrentFrameBackwards()
    {
      MoveFrameBackwards(CurrentFrame);
    }

    public void DeleteFrame(GestureFrame f)
    {
      if (f == null) return;

      Gesture g = (Gesture)TheWorkspace.DataContext;
      if (g == null) return;

      g.Frames.Remove(f);

      if (g.Frames.Count == 0)
        g.Frames.Add(MakeNewGestureFrame()); // Do not let a gesture have 0 frames //TODO: having 0-framed gestures could be useful for creating named libraries of keyboard shortcuts as templates (say one library for PowerPoint, one for Media Player etc.) to which a user can then fill-in the gesture frames (would need the Visualizer to be aware of that and also maybe show the respective gestures colored as red - to signify they're not yet complete)

      if (FramesListBox.SelectedItem == null)
        FramesListBox.SelectedIndex = FramesListBox.Items.Count - 1;

      SyncEditorGrids();
    }

    public void MoveFrameForward(GestureFrame f)
    {
      if (f == null) return;

      Gesture g = (Gesture)TheWorkspace.DataContext;
      if (g == null) return;

      int fIndex = g.Frames.IndexOf(f);
      if (fIndex == g.Frames.Count - 1)
        g.Frames.Move(fIndex, 0);
      else
        g.Frames.Move(fIndex, fIndex + 1);

      SyncEditorGrids();
    }

    public void MoveFrameBackwards(GestureFrame f)
    {
      if (f == null) return;

      Gesture g = (Gesture)TheWorkspace.DataContext;
      if (g == null) return;

      int fIndex = g.Frames.IndexOf(f);
      if (fIndex == 0)
        g.Frames.Move(fIndex, g.Frames.Count - 1);
      else
        g.Frames.Move(fIndex, fIndex - 1);

      SyncEditorGrids();
    }

    #endregion

    private void EnableKinect_Editor()
    {
      if (kinect != null)
      {
        kinect.SkeletonStream.Enable();
        kinect.SkeletonFrameReady += Kinect_SkeletonFrameReady_Editor;
        //kinect.Start(); //Kinect is also used for speech recognition if available, so starting at launch and stopping at end of app
      }
    }

    private void DisableKinect_Editor()
    {
      if (kinect != null)
      {
        //kinect.Stop(); //Kinect is also used for speech recognition if available, so starting at launch and stopping at end of app
        kinect.SkeletonFrameReady -= Kinect_SkeletonFrameReady_Editor;
        kinect.SkeletonStream.Disable();
      }
    }

    #region Sync UI Grids

    /// <summary>
    /// Enable/disable rows on SideViewGrid according to selection on FrontViewGrid
    /// </summary>
    private void SyncEditorGrids_FrontToSide()
    {
      GestureFrame sf = (GestureFrame)FramesListBox.SelectedItem;
      GestureFrameCell[] fcs = (GestureFrameCell[])FVGrid.ItemsSource;
      GestureFrameCell[] scs = (GestureFrameCell[])SVGrid.ItemsSource;

      // 'If' overcomes FVGrid_SelectionChanged firing before everything else and syncing SVGrid to a different Frame's FVGrid
      if (Object.ReferenceEquals(sf.FrontCells, fcs) && Object.ReferenceEquals(sf.SideCells, scs))
      {
        IList frontViewGrid_selectedCells = (IList)FVGrid.SelectedItems;
        List<int> frontViewGrid_selectedRows = new List<int>();

        foreach (GestureFrameCell c in frontViewGrid_selectedCells)
          frontViewGrid_selectedRows.Add((int)(c.IndexInFrame / 20));

        for (int i = 0; i < 400; i++)
        {
          ListBoxItem sideViewGridItemContainer = (ListBoxItem)SVGrid.ItemContainerGenerator.ContainerFromIndex(i);
          if (frontViewGrid_selectedRows.Contains((int)(i / 20))) sideViewGridItemContainer.IsEnabled = true;
          else
          {
            sideViewGridItemContainer.IsEnabled = false;
            GestureFrame f = (GestureFrame)SVGrid.DataContext;
            f.SideCells[i].IsHotspot = false;
          }
        }

      }
    }

    /// <summary>
    /// Put hints for the current gesture's existing hotspots below both 2D grids
    /// </summary>
    private void SyncEditorGrids_2D_Hints(Gesture g)
    {
      FVHints.Children.Clear();
      SVHints.Children.Clear();
      for (int i = 0; i < 400; i++)
      {
        FVHints.Children.Add(new Border() { Background = Brushes.Transparent, BorderBrush = Brushes.Transparent });
        SVHints.Children.Add(new Border() { Background = Brushes.Transparent, BorderBrush = Brushes.Transparent });
      }
      foreach (GestureFrame f in g.Frames)
      {
        foreach (GestureFrameCell c in f.FrontCells.Where(c => c.IsHotspot))
        {
          Border b = (Border)FVHints.Children[c.IndexInFrame];
          b.Background = new SolidColorBrush() { Color = Colors.SlateBlue, Opacity = 0.2 };
        }

        foreach (GestureFrameCell c in f.SideCells.Where(c => c.IsHotspot))
        {
          Border b = (Border)SVHints.Children[c.IndexInFrame];
          b.Background = new SolidColorBrush() { Color = Colors.SlateBlue, Opacity = 0.2 };
        }
      }
    }

    /// <summary>
    /// Sync 3D Viewport with grids
    /// </summary>
    private void SyncEditorGrids_3D(Gesture g) //TODO: split into smaller methods
    {
      // Init 3D stuff
      Model3DGroup modelGroup = new Model3DGroup();
      foreach (GestureFrame f in g.Frames)
      {
        // Create material
        SolidColorBrush materialBrush = new SolidColorBrush()
        {
          Color = Colors.DarkSlateBlue,
          Opacity = 0.1 + ((double)(g.Frames.IndexOf(f) + 1) / (double)g.Frames.Count) * 0.8
        };
        DiffuseMaterial material = new DiffuseMaterial(materialBrush);

        foreach (GestureFrameCell fc in f.FrontCells.Where(fc => fc.IsHotspot == true))
        {
          int fcIndex = Array.IndexOf(f.FrontCells, fc);
          foreach (GestureFrameCell sc in f.SideCells.Where(
                   sc => sc.IsHotspot == true && (int)(Array.IndexOf(f.SideCells, sc) / 20) == (int)(fcIndex / 20)))
          {
            // Init mesh
            MeshBuilder meshBuilder = new MeshBuilder(false, false);
            // Make cube and add to mesh
            double y = (fc.LeftCM + fc.RightCM) / 2;
            double z = (fc.TopCM + fc.BottomCM) / 2;
            double x = (sc.LeftCM + sc.RightCM) / 2;
            Point3D cubeCenter = new Point3D(x, y, z);
            meshBuilder.AddBox(cubeCenter, 15, 15, 15);

            // Create and freeze mesh
            var mesh = meshBuilder.ToMesh(true);

            // Create model
            modelGroup.Children.Add(new GeometryModel3D(mesh, material));
          }
        }
      }

      // Suggest other gestures too
      foreach (Gesture gg in GestureCollection)
      {
        foreach (GestureFrame f in gg.Frames)
        {
          // Create material
          SolidColorBrush materialBrush = new SolidColorBrush()
          {
            Color = Visualizer_GestureColors[GestureCollection.IndexOf(gg) % Visualizer_GestureColors.Length].Color,
            Opacity = ((double)(gg.Frames.IndexOf(f) + 1) / (double)gg.Frames.Count) * 0.09
          };
          DiffuseMaterial material = new DiffuseMaterial(materialBrush);

          foreach (GestureFrameCell fc in f.FrontCells.Where(fc => fc.IsHotspot == true))
          {
            int fcIndex = Array.IndexOf(f.FrontCells, fc);
            foreach (GestureFrameCell sc in f.SideCells.Where(
                sc => sc.IsHotspot == true && (int)(Array.IndexOf(f.SideCells, sc) / 20) == (int)(fcIndex / 20)))
            {
              // Init mesh
              MeshBuilder meshBuilder = new MeshBuilder(false, false);

              // Make cube and add to mesh
              double y = (fc.LeftCM + fc.RightCM) / 2;
              double z = (fc.TopCM + fc.BottomCM) / 2;
              double x = (sc.LeftCM + sc.RightCM) / 2;
              Point3D cubeCenter = new Point3D(x, y, z);
              meshBuilder.AddBox(cubeCenter, 15, 15, 15);

              // Create and freeze mesh
              var mesh = meshBuilder.ToMesh(true);

              // Create model
              modelGroup.Children.Add(new GeometryModel3D(mesh, material));
            }
          }
        }
      }

      HotspotCellsModelVisual3D_Editor.Content = modelGroup;
    }

    private void SyncEditorGrids()
    {
      try
      {
        SyncEditorGrids_FrontToSide();
        Gesture g = (Gesture)TheWorkspace.DataContext;
        SyncEditorGrids_2D_Hints(g);
        SyncEditorGrids_3D(g);
      }
      catch (NullReferenceException) { return; }
    }

    #endregion

    #endregion

    #region --- Events ---

    #region Command selection UI

    private void SetCommandButton_Click(object sender, RoutedEventArgs e)
    {
      // Put the overlay on Editor
      EditorSetCommandOverlayKeyText.DataContext = TheWorkspace.DataContext;
      EditorSetCommandOverlay.Visibility = Visibility.Visible;

      // Save the current command in case of cancel
      Gesture g = (Gesture)TheWorkspace.DataContext;
      CommandBackup = new ObservableCollection<Key>(g.Command);

      // Listen for key press
      AddHandler(Keyboard.PreviewKeyDownEvent, (KeyEventHandler)SetCommand_KeyDown);
      AddHandler(Keyboard.PreviewKeyUpEvent, (KeyEventHandler)SetCommand_KeyUp);
    }

    private void SetCommand_KeyDown(object sender, KeyEventArgs e)
    {
      Key k = (e.Key == Key.System ? e.SystemKey : e.Key);
      if (KeysBeingPressed.Contains(k))
        return;
      else
        KeysBeingPressed.Add(k);

      Gesture g = (Gesture)TheWorkspace.DataContext;
      g.Command = new ObservableCollection<Key>(KeysBeingPressed);

      e.Handled = true;
    }

    private void SetCommand_KeyUp(object sender, KeyEventArgs e)
    {
      Key k = (e.Key == Key.System ? e.SystemKey : e.Key);
      KeysBeingPressed.Remove(k);

      e.Handled = true;
    }

    #endregion

    #region Joint UI

    private void JointSelector_Checked(object sender, RoutedEventArgs e)
    {
      JointTitle.Text = ((RadioButton)sender).ToolTip.ToString();
    }

    #endregion

    #region Frames UI

    private void AddNewFrameButton_Click(object sender, RoutedEventArgs e)
    {
      AddNewFrame();
    }

    private void SelectPreviousFrameButton_Click(object sender, RoutedEventArgs e)
    {
      SelectPreviousFrame();
    }

    private void SelectNextFrameButton_Click(object sender, RoutedEventArgs e)
    {
      SelectNextFrame();
    }

    private void DeleteFrameButton_Click(object sender, RoutedEventArgs e) //reused at each frame
    {
      Button b = (Button)sender; //find the button clicked
      GestureFrame s = (GestureFrame)b.DataContext; //find the corresponding frame
      DeleteFrame(s);
    }

    private void MoveFrameBackwardButton_Click(object sender, RoutedEventArgs e) //reused at each frame
    {
      Button b = (Button)sender; //find the button clicked
      GestureFrame f = (GestureFrame)b.DataContext; //find the corresponding frame
      MoveFrameBackwards(f);
    }

    private void MoveFrameForwardButton_Click(object sender, RoutedEventArgs e) //reused at each frame
    {
      Button b = (Button)sender; //find the button clicked
      GestureFrame f = (GestureFrame)b.DataContext; //find the corresponding frame
      MoveFrameForward(f);
    }

    #endregion

    #region Front and Side view UI

    /// <summary>
    /// Enable/disable rows on SideViewGrid according to selection on FrontViewGrid
    /// </summary>
    private void FVGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SyncEditorGrids();
    }

    private void SVGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SyncEditorGrids();
    }

    #endregion

    private void Kinect_SkeletonFrameReady_Editor(object sender, SkeletonFrameReadyEventArgs e)
    {
      //TODO: check: original code had these as separate event handlers, could it be for them to be potentially parallelized? (guess not)
      SkeletonFrameReady_Draw3D_Editor(e);
      SkeletonFrameReady_Draw3D_Front_Editor(e);
      SkeletonFrameReady_Draw3D_Side_Editor(e);
      SkeletonFrameReady_ToggleBackground_Editor(e);
      SkeletonFrameReady_Detect_Editor(e);
    }

    #endregion

  }
}
