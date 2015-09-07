//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.xaml.cs
//Version: 20150907

using Hotspotizer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFLocalizeExtension.Engine;
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
      LoadPlugins();
      RegisterCommands();
      GestureCollection = new ObservableCollection<Gesture>();
      InitializeComponent();
      InitLocalization(); //must be called after "InitializeComponent"

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
      kinect = GetKinectSensor();

      // KinectErrorStackPanel that displays errors & warnings is shown if Kinect isn't connected
      KinectErrorStackPanel.Visibility = (kinect == null)? Visibility.Visible : Visibility.Hidden;

      // Speak out about missing Kinect sensor
      if (kinect == null)
        speechSynthesis?.Speak(GlblRes.ResourceManager.GetString("KinectNotDetected", speechSynthesis.Culture)); //speech culture may not be the same as UI culture if for example only en-US voices are available
    }

    #endregion

    #region --- Properties ---

    public ObservableCollection<Gesture> GestureCollection { get; set; } // The gesture collection and gesture we are operating on

    #endregion

    #region --- Methods ---

    #region Localization

    private void InitLocalization()
    {
      LocalizeDictionary.Instance.SetCurrentThreadCulture = true; //when changing localization language will also automatically change the current culture

      LocalizeDictionary.Instance.Culture = Thread.CurrentThread.CurrentCulture; //default to the current culture as set by the OS when launching the app...
      //...to force a specific culture (say "en") instead, use:
      //LocalizeDictionary.Instance.Culture = new CultureInfo("en");

      UpdateSelectorFromLanguage();
    }

    /// <summary>
    /// Updates the language selection UI from the LocalizeDictionary's language
    /// </summary>
    private void UpdateSelectorFromLanguage()
    {
      comboLanguage.SelectedValue = LocalizeDictionary.Instance.Culture.TwoLetterISOLanguageName;
    }

    /// <summary>
    /// Updates the LocalizeDictionary's language from the current selection at the language selection UI
    /// </summary>
    private void UpdateLanguageFromSelector()
    {
      string s = comboLanguage.SelectedValue as string;
      if (s == null) return;
      CultureInfo culture = CultureInfo.CreateSpecificCulture(s);
      if (culture != null)
        LocalizeDictionary.Instance.Culture = culture;
    }

    #endregion

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

    private void sdkDownloadLink_MouseDown(object sender, MouseButtonEventArgs e)
    {
      System.Diagnostics.Process.Start(GlblRes.URL_DownloadKinectSDK);
    }

    private void OuterMostGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      var g = (Grid)sender;
      Double maxW = e.NewSize.Width - g.ColumnDefinitions[1].MinWidth - g.ColumnDefinitions[0].ActualWidth;
      g.ColumnDefinitions[0].MaxWidth = maxW;
    }

    private void comboLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateLanguageFromSelector();
    }

    #endregion

  }
}
