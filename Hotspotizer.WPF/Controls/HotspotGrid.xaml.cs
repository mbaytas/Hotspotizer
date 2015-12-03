//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: HotspotGrid.cs
//Version: 20150817

using System.Windows.Controls;
using System.Windows.Input;
using Hotspotizer.Models;

namespace Hotspotizer.Controls
{
  public partial class HotspotGrid : ListBox
  {

    #region --- Initialization ---

    public HotspotGrid()
    {
      InitializeComponent();
    }

    #endregion

    #region --- Evens ---

    private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      foreach (GestureFrameCell c in this.Items)
      {
        ListBoxItem lbi = (ListBoxItem)this.ItemContainerGenerator.ContainerFromItem(c);
        lbi.MouseEnter += ListBoxItem_MouseEnter;
      }
    }

    private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      foreach (GestureFrameCell c in this.Items)
      {
        ListBoxItem lbi = (ListBoxItem)this.ItemContainerGenerator.ContainerFromItem(c);
        lbi.MouseEnter -= ListBoxItem_MouseEnter;
      }
    }

    private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
    {
      ListBoxItem lbi = (ListBoxItem)sender;
      lbi.IsSelected = !lbi.IsSelected;
    }

    #endregion

  }
}
