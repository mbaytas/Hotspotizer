//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.ManagerUtils.cs
//Version: 20150817

using System.Windows;
using System.Windows.Controls;
using Hotspotizer.Models;
using GlblRes = Hotspotizer.Properties.Resources;

namespace Hotspotizer
{
  public partial class MainWindow
  {

    #region --- Events ---

    public void DeleteGestureButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;
      if (MessageBox.Show(string.Format(GlblRes.DeleteGestureFromCollectionConfirmation, g.Name),
                          GlblRes.DeleteGestureFromCollection, MessageBoxButton.YesNo) 
          == MessageBoxResult.Yes)
        GestureCollection.Remove(g);
    }

    public void EditGestureButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;

      // Store the initial state of the gesture being edited
      ExGesture = DeepCopyGesture(g);
      // Go go go
      TheWorkspace.DataContext = g;
      LaunchEditor();
    }

    #endregion

  }
}
