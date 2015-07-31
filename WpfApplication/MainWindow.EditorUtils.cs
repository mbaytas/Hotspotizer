using HelixToolkit.Wpf;
using Microsoft.Kinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApplication.Helpers;
using WpfApplication.Models;

namespace WpfApplication
{
  public partial class MainWindow
  {

    #region --- Fields ---

    // Stores the initial state of the gesture being edited
    Gesture ExGesture = null;

    // Stores the initial state of the Gesture.Command being edited
    ObservableCollection<Key> CommandBackup = new ObservableCollection<Key>();
    // Stores the keys being pressed while editing a Gesture.Command
    List<Key> KeysBeingPressed = new List<Key>();

    #endregion

    #region --- Methods ---

    private void LaunchEditor()
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
      // Enable Kinect
      if (kinect != null)
      {
        kinect.SkeletonStream.Enable();
        kinect.SkeletonFrameReady += SkeletonFrameReady_Draw3D_Editor;
        kinect.SkeletonFrameReady += SkeletonFrameReady_Draw3D_Editor;
        kinect.SkeletonFrameReady += SkeletonFrameReady_Draw3D_Front_Editor;
        kinect.SkeletonFrameReady += SkeletonFrameReady_Draw3D_Side_Editor;
        kinect.SkeletonFrameReady += SkeletonFrameReady_ToggleBackground_Editor;
        kinect.SkeletonFrameReady += SkeletonFrameReady_Detect_Editor;
        kinect.Start();
      }
      // Show Editor
      TheEditor.Visibility = Visibility.Visible;
      EditorOverlay.Visibility = Visibility.Hidden;
      // Hide Manager
      ManagerOverlay.Visibility = Visibility.Visible;
      // Reset Frame selection
      FramesListBox.SelectedIndex = 0;
      // Kill keyboard control on 3D grid
      EventLogic.RemoveRoutedEventHandlers(ViewPort3D_Editor.CameraController, HelixToolkit.Wpf.CameraController.KeyDownEvent);
    }

    private void KillEditor()
    {
      // Disable Kinect
      if (kinect != null)
      {
        kinect.Stop();
        kinect.SkeletonFrameReady -= SkeletonFrameReady_Draw3D_Editor;
        kinect.SkeletonFrameReady -= SkeletonFrameReady_Draw3D_Front_Editor;
        kinect.SkeletonFrameReady -= SkeletonFrameReady_Draw3D_Side_Editor;
        kinect.SkeletonFrameReady -= SkeletonFrameReady_ToggleBackground_Editor;
        kinect.SkeletonFrameReady -= SkeletonFrameReady_Detect_Editor;
        kinect.SkeletonStream.Disable();
      }
      // Clean up DataContext
      TheWorkspace.DataContext = InitGesture;
      // Hide Editor
      EditorOverlay.Visibility = Visibility.Visible;
      // Show Manager
      ManagerOverlay.Visibility = Visibility.Hidden;
    }

    // Enable/disable rows on SideViewGrid according to selection on FrontViewGrid
    private void SyncEditorGrids_FrontToSide(Gesture g)
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
        {
          frontViewGrid_selectedRows.Add((int)(c.IndexInFrame / 20));
        }
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

    // Put hints for the current gesture's existing hotspots below both 2D grids
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

    // Also sync 3D Viewport with grids
    private void SyncEditorGrids_3D(Gesture g)
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
        Gesture g = (Gesture)TheWorkspace.DataContext;

        SyncEditorGrids_FrontToSide(g);
        SyncEditorGrids_2D_Hints(g);
        SyncEditorGrids_3D(g);
      }
      catch (NullReferenceException) { return; }
    }

    #endregion

    #region --- Events ---

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
      Gesture g = (Gesture)TheWorkspace.DataContext;
      Key k = (e.Key == Key.System ? e.SystemKey : e.Key);
      if (KeysBeingPressed.Contains(k)) return;
      else KeysBeingPressed.Add(k);
      g.Command = new ObservableCollection<Key>(KeysBeingPressed);
      e.Handled = true;
    }

    private void SetCommand_KeyUp(object sender, KeyEventArgs e)
    {
      Key k = (e.Key == Key.System ? e.SystemKey : e.Key);
      KeysBeingPressed.Remove(k);
      e.Handled = true;
    }

    private void SetCommandCancelButton_Click(object sender, RoutedEventArgs e)
    {
      // Clean up
      RemoveHandler(Keyboard.PreviewKeyDownEvent, (KeyEventHandler)SetCommand_KeyDown);
      RemoveHandler(Keyboard.PreviewKeyUpEvent, (KeyEventHandler)SetCommand_KeyUp);
      EditorSetCommandOverlay.Visibility = Visibility.Hidden;
      // De-update gesture
      Gesture g = (Gesture)TheWorkspace.DataContext;
      g.Command = new ObservableCollection<Key>(CommandBackup);
    }

    private void SetCommandOKButton_Click(object sender, RoutedEventArgs e)
    {
      // Clean up
      RemoveHandler(Keyboard.PreviewKeyDownEvent, (KeyEventHandler)SetCommand_KeyDown);
      RemoveHandler(Keyboard.PreviewKeyUpEvent, (KeyEventHandler)SetCommand_KeyUp);
      EditorSetCommandOverlay.Visibility = Visibility.Hidden;
    }

    private void AddNewFrameButton_Click(object sender, RoutedEventArgs e)
    {
      Gesture g = (Gesture)TheWorkspace.DataContext;
      // Add the frame to the Gesture object
      g.Frames.Add(MakeNewGestureFrame());
      // Extend CollisionTimes to keep track of collisions
      CollisionTimes[GestureCollection.IndexOf(g)].Add(new DateTime());
      // Select the newly added frame on the timeline
      FramesListBox.SelectedIndex = FramesListBox.Items.Count - 1;
      SyncEditorGrids();
    }

    private void DeleteFrameButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      GestureFrame s = (GestureFrame)b.DataContext;
      Gesture g = (Gesture)TheWorkspace.DataContext;
      g.Frames.Remove(s);
      if (g.Frames.Count == 0) g.Frames.Add(MakeNewGestureFrame()); // Do not let a gesture have 0 frames
      if (FramesListBox.SelectedItem == null) FramesListBox.SelectedIndex = FramesListBox.Items.Count - 1;
      SyncEditorGrids();
    }

    private void MoveFrameForwardButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      GestureFrame f = (GestureFrame)b.DataContext;
      Gesture g = (Gesture)TheWorkspace.DataContext;
      int fIndex = g.Frames.IndexOf(f);
      if (fIndex == g.Frames.Count - 1) g.Frames.Move(fIndex, 0);
      else g.Frames.Move(fIndex, fIndex + 1);
      SyncEditorGrids();
    }

    private void MoveFrameBackwardButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      GestureFrame f = (GestureFrame)b.DataContext;
      Gesture g = (Gesture)TheWorkspace.DataContext;
      int fIndex = g.Frames.IndexOf(f);
      if (fIndex == 0) g.Frames.Move(fIndex, g.Frames.Count - 1);
      else g.Frames.Move(fIndex, fIndex - 1);
      SyncEditorGrids();
    }

    // Enable/disable rows on SideViewGrid according to selection on FrontViewGrid
    private void FVGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SyncEditorGrids();
    }

    private void SVGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SyncEditorGrids();
    }

    #endregion

  }
}
