using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApplication.Models;

namespace WpfApplication {
  public partial class MainWindow {

    public void DeleteGestureButton_Click(object sender, RoutedEventArgs e) {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;
      if (MessageBox.Show("Do you really want to delete the gesture \"" + g.Name + "\" from the collection?",
          "Delete Gesture from Collection",
          MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        GestureCollection.Remove(g);
    }

    public void EditGestureButton_Click(object sender, RoutedEventArgs e) {
      Button b = (Button)sender;
      Gesture g = (Gesture)b.DataContext;
      // Store the initial state of the gesture being edited
      ExGesture = DeepCopyGesture(g);
      // Go go go
      TheWorkspace.DataContext = g;
      LaunchEditor();
    }

  }
}
