//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: MainWindow.SpeechUtils.cs
//Version: 20151208

using Hotspotizer.Models;
using Hotspotizer.Plugins;
using SpeechLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Input;

namespace Hotspotizer
{
  public partial class MainWindow : Window
  {
    #region --- Constants ---

    private const double DEFAULT_SPEECH_RECOGNITION_CONFIDENCE_THRESHOLD = 0.7;
    private const char GESTURE_NAME_SPEECH_COMMAND_SEPARATOR = '-';

    #endregion

    #region --- Fields ---

    public ISpeechSynthesis speechSynthesis;
    public ISpeechRecognition speechRecognition;

    private double SpeechRecognitionConfidenceThreshold = DEFAULT_SPEECH_RECOGNITION_CONFIDENCE_THRESHOLD;
    private string grammarsFolder; //=null
    private Grammar gestureCollectionGrammar; //=null

    #endregion

    #region --- Methods ---

    public void LoadSpeechSynthesisPlugin()
   {
     Lazy<ISpeechSynthesis> plugin = PluginsCatalog.mefContainer.GetExports<ISpeechSynthesis>("SpeechLib.Synthesis").FirstOrDefault();
     speechSynthesis = (plugin != null) ? plugin.Value : null;
   }

    public void LoadSpeechRecognitionPlugin()
    {
      Lazy<ISpeechRecognitionKinect> plugin1 = PluginsCatalog.mefContainer.GetExports<ISpeechRecognitionKinect>("SpeechLib.Recognition.KinectV1").FirstOrDefault();
      speechRecognition = (plugin1 != null) ? plugin1.Value : null;

      if (speechRecognition == null) //SpeechRecognitionKinect plugin couldn't be loaded, try to fallback to the SpeechRecognition one (which uses the default audio source as input)
      {
        Lazy<ISpeechRecognition> plugin2 = PluginsCatalog.mefContainer.GetExports<ISpeechRecognition>("SpeechLib.Recognition").FirstOrDefault();
        speechRecognition = (plugin2 != null) ? plugin2.Value : null;
      }

      if (speechRecognition != null)
        StartSpeechRecognition();
    }

    #region Grammars

    private void LoadSpeechRecognitionGrammarsForUI() //TODO: split into separate methods and load based on current context, instead of loading all together
    {
      speechRecognition.Pause();
      LoadSpeechRecognitionGrammar("SpeechGrammar_Manager_en.xml", "Manager");
      LoadSpeechRecognitionGrammar("SpeechGrammar_Editor_en.xml", "Editor");
      LoadSpeechRecognitionGrammar("SpeechGrammar_Visualizer_en.xml", "Visualizer");
      speechRecognition.Resume();
    }

    private void LoadSpeechRecognitionGrammar(string filename, string name)
    {
      if (grammarsFolder == null)
        grammarsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Grammars", "SRGS");

      speechRecognition.LoadGrammar(new FileStream(Path.Combine(grammarsFolder, filename), FileMode.Open), name);
    }

    private void LoadSpeechRecognitionGrammarForGestureCollection()
    {
      speechRecognition.Pause();
      UnloadSpeechRecognitionGrammarForGestureCollection();
      speechRecognition.LoadGrammar(gestureCollectionGrammar = CreateGrammarForGestureCollection(GestureCollection));
      speechRecognition.Resume();
    }

    private void UnloadSpeechRecognitionGrammarForGestureCollection()
    {
      if (gestureCollectionGrammar != null)
      {
        speechRecognition.UnloadGrammar(gestureCollectionGrammar);
        gestureCollectionGrammar = null;
      }
    }

    public static Grammar CreateGrammarForGestureCollection(ObservableCollection<Gesture> gestures)
    {
      var commands = new Choices();
      foreach (Gesture g in gestures)
      {
        string gestureName = g.Name;
        string gestureSpeechCommand = g.Name.Split(GESTURE_NAME_SPEECH_COMMAND_SEPARATOR)[0].Trim(); //keep the first part only (after splitting the string) if the name of the gesture contains "-"
        commands.Add(new SemanticResultValue(gestureSpeechCommand, g));
      }

      var gb = new GrammarBuilder { Culture = CultureInfo.GetCultureInfoByIetfLanguageTag("en") };
      gb.Append(commands);

      return new Grammar(gb);
    }

    #endregion

    protected void StartSpeechRecognition() //called by LoadSpeechRecognitionPlugin
    {
      if (speechRecognition == null)
        return;

      try
      {
        LoadSpeechRecognitionGrammarsForUI();
        LoadSpeechRecognitionGrammarForGestureCollection();

        GestureCollectionLoaded += MainWindow_GestureCollectionLoaded;
        speechRecognition.Recognized += SpeechRecognition_Recognized;
        speechRecognition.NotRecognized += SpeechRecognition_NotRecognized;

        /*
        //For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model.
        //This will prevent recognition accuracy from degrading over time.
        speechRecognition.AcousticModelAdaptation = false;
        */

        speechRecognition.Start();
      }
      catch(Exception e)
      {
        speechRecognition = null;
        MessageBox.Show(e.Message);
      }
    }

    #endregion

    #region --- Events ---

    private void SpeechRecognition_Recognized(object sender, SpeechRecognitionEventArgs e)
    {
      Dictionary<string, ICommand> commands;
      if (EditorVisible) commands = commands_Editor;
      else if (VisualizerVisible) commands = commands_Visualizer;
      else commands = commands_Manager;

      ICommand cmd;
      if ((e.confidence >= SpeechRecognitionConfidenceThreshold) &&
          commands.TryGetValue(e.command, out cmd) &&
          cmd.CanExecute(null)
         ) //must use && to do short-circuit boolean evaluation here
        cmd.Execute(null);
    }

    private void SpeechRecognition_NotRecognized(object sender, EventArgs e)
    {
      //TODO: maybe show some hint about which are the supported voice commands
    }

    private void MainWindow_GestureCollectionLoaded(object sender, EventArgs e)
    {
      LoadSpeechRecognitionGrammarForGestureCollection(); //this will also unload the Grammar generated for the previous gesture collection
    }

    #endregion

  }
}
