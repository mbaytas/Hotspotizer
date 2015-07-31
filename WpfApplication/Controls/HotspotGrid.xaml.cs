using System.Windows.Controls;
using System.Windows.Input;
using WpfApplication.Models;

namespace WpfApplication.Controls
{
  public partial class HotspotGrid : ListBox
  {

    public HotspotGrid()
    {
      InitializeComponent();
    }

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

  }
}
