//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: App.xaml.cs
//Version: 20151208

using Microsoft.Kinect;
using System;
using System.Linq;
using System.Windows;

namespace Hotspotizer {
  public partial class App : Application
  {
    // http://channel9.msdn.com/coding4fun/kinect/When-deploying-Kinect-apps-ensure-the-runtime-components-and-device-are-ready

    protected override void OnStartup(StartupEventArgs e)
    {
      if (IsKinectRuntimeInstalled)
        base.OnStartup(e);
      else if (MessageBoxResult.OK == MessageBox.Show(
                "Hotspotizer requires Microsoft Kinect Runtime 1.8 to run.\nClick \"OK\" to download Microsoft Kinect Runtime 1.8 from Microsoft's website.",
                "Kinect Runtime required",
                MessageBoxButton.OKCancel)
              )
              System.Diagnostics.Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=40277");
    }

    public bool IsKinectRuntimeInstalled {
      get
      {
        bool isInstalled;
        try
        {
          TestForKinectTypeLoadException();
          isInstalled = true;
        }
        catch (TypeInitializationException)
        {
          isInstalled = false;
        }
        return isInstalled;
      }
    }

    private void TestForKinectTypeLoadException()
    {
      KinectSensor kinectCheck = KinectSensor.KinectSensors.FirstOrDefault();
    }

  }
}
