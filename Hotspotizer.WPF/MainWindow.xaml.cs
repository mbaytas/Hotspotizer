//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.xaml.cs
//Version: 20150817

using Microsoft.Kinect;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hotspotizer.Models;
using GlblRes = Hotspotizer.Properties.Resources;

namespace Hotspotizer
{
  public partial class MainWindow : Window
  {

    #region --- Fields ---

    Gesture InitGesture = new Gesture(); // Needed for Editor's data bindings to come alive

    #endregion

    #region --- Initialization ---

    public MainWindow()
    {
      registerCommands();

      GestureCollection = new ObservableCollection<Gesture>();

      InitializeComponent();

      DependencyPropertyDescriptor.FromProperty(Controls.HotspotGrid.ItemsSourceProperty, typeof(Controls.HotspotGrid)).
        AddValueChanged(FVGrid, (s, e) =>
        {
          if (SVGrid.ItemsSource != null && FVGrid.ItemsSource != null)
            SyncEditorGrids();
        });

      DependencyPropertyDescriptor.FromProperty(Controls.HotspotGrid.ItemsSourceProperty, typeof(Controls.HotspotGrid)).
        AddValueChanged(SVGrid, (s, e) =>
        {
          if (SVGrid.ItemsSource != null && FVGrid.ItemsSource != null)
            SyncEditorGrids();
        });

      //EditorTipsOverlay.Visibility = Visibility.Visible;
      EditorOverlay.Visibility = Visibility.Visible;

      // Holla Kinect
      kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

      // KinectErrorStackPanel that displays errors & warnings is shown if Kinect isn't connected
      KinectErrorStackPanel.Visibility = (kinect == null)? Visibility.Visible : Visibility.Hidden;
    }

    #endregion

    #region --- Properties ---

    public ObservableCollection<Gesture> GestureCollection { get; set; } // The gesture collection and gesture we are operating on

    #endregion

    #region --- Methods ---

    private Gesture MakeNewGesture()
    {
      return new Gesture()
      {
        Command = new ObservableCollection<Key>() { Key.None },
        Frames = new ObservableCollection<GestureFrame> { MakeNewGestureFrame() }
      };
    }

    private GestureFrame MakeNewGestureFrame()
    {
      GestureFrame newFrame = new GestureFrame();
      for (int i = 0; i < 400; i++)
      {
        newFrame.FrontCells[i] = new GestureFrameCell() { IndexInFrame = i, IsHotspot = false };
        newFrame.SideCells[i] = new GestureFrameCell() { IndexInFrame = i, IsHotspot = false };
      }
      return newFrame;
    }

    private GestureFrame DeepCopyGestureFrame(GestureFrame sourceFrame)
    {
      GestureFrame targetFrame = new GestureFrame();
      for (int i = 0; i < 400; i++)
      {
        targetFrame.FrontCells[i] = new GestureFrameCell()
        {
          IndexInFrame = sourceFrame.FrontCells[i].IndexInFrame,
          IsHotspot = sourceFrame.FrontCells[i].IsHotspot
        };
        targetFrame.SideCells[i] = new GestureFrameCell()
        {
          IndexInFrame = sourceFrame.SideCells[i].IndexInFrame,
          IsHotspot = sourceFrame.SideCells[i].IsHotspot
        };
      }
      return targetFrame;
    }

    private Gesture DeepCopyGesture(Gesture source)
    {
      Gesture target = new Gesture();

      target.Name = source.Name;
      target.Command = new ObservableCollection<Key>(source.Command);
      target.Hold = source.Hold; // System.Boolean's value type is bool
      target.Joint = source.Joint; // Enums are also value type

      target.Frames = new ObservableCollection<GestureFrame>();
      foreach (GestureFrame sourceFrame in source.Frames)
        target.Frames.Add(DeepCopyGestureFrame(sourceFrame));

      return target;
    }

    #endregion

    #region --- Events ---

    void sdkDownloadLink_MouseDown(object sender, MouseButtonEventArgs e)
    {
      System.Diagnostics.Process.Start(GlblRes.URL_DownloadKinectSDK);
    }

    private void OuterMostGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      var g = (Grid)sender;
      Double maxW = e.NewSize.Width - g.ColumnDefinitions[1].MinWidth - g.ColumnDefinitions[0].ActualWidth;
      g.ColumnDefinitions[0].MaxWidth = maxW;
    }

    #endregion

  }
}
