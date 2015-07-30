using Microsoft.Kinect;
using System;
using System.Linq;
using System.Windows;

namespace WpfApplication {
  public partial class App : Application {
    // http://channel9.msdn.com/coding4fun/kinect/When-deploying-Kinect-apps-ensure-the-runtime-components-and-device-are-ready
    protected override void OnStartup(StartupEventArgs e) {
      if (IsKinectRuntimeInstalled) {
        base.OnStartup(e);
      }
      else {
        MessageBoxResult result = MessageBox.Show("Hotspotizer requires Microsoft Kinect Runtime 1.8 to run.\nClick \"OK\" to download Microsoft Kinect Runtime 1.8 from Microsoft's website.",
            "Kinect Runtime required",
            MessageBoxButton.OKCancel);
        if (result == MessageBoxResult.OK) {
          System.Diagnostics.Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=40277");
        }
      }
    }

    public bool IsKinectRuntimeInstalled {
      get {
        bool isInstalled;
        try {
          TestForKinectTypeLoadException();
          isInstalled = true;
        }
        catch (TypeInitializationException) {
          isInstalled = false;
        }
        return isInstalled;
      }
    }

    private void TestForKinectTypeLoadException() {
      KinectSensor kinectCheck = KinectSensor.KinectSensors.FirstOrDefault();
    }
  }
}
