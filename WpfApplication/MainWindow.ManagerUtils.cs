//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.ManagerUtils.cs
//Version: 20150809

using System.Windows;
using System.Windows.Controls;
using WpfApplication.Models;

namespace WpfApplication
{
  public partial class MainWindow
  {

    #region --- Events ---

    public void DeleteGestureButton_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;
      if (MessageBox.Show(string.Format("Do you really want to delete the gesture \"%s\" from the collection?", g.Name),
                          "Delete Gesture from Collection", MessageBoxButton.YesNo) 
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
