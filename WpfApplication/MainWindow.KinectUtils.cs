//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.KinectUtils.cs
//Version: 20150731

using HelixToolkit.Wpf;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WindowsInput;
using WindowsInput.Native;
using WpfApplication.Models;

namespace WpfApplication
{
  public partial class MainWindow : Window
  {

    #region --- Fields ---

    // Kinect-related members
    private KinectSensor kinect = null;
    // Init input simulator
    private InputSimulator inputSimulator = new InputSimulator();

    // Visuals
    SolidColorBrush[] Visualizer_GestureColors = {
      Brushes.Red,
      Brushes.Green,
      Brushes.Blue,
      Brushes.Yellow,
      Brushes.Magenta,
      Brushes.Cyan,
    };
    Model3DGroup CollisionHighlights_3D;
    Model3DGroup CollisionHighlights_Front;
    Model3DGroup CollisionHighlights_Side;

    // Fucking gesture recognition shit
    enum JointCollisionStates { InHotspot, InUltimateHotspot, OutThere };
    List<JointCollisionStates[]> CollisionStates;
    List<List<DateTime>> CollisionTimes;
    TimeSpan CollisionTimeout = TimeSpan.FromMilliseconds(500);

    #endregion

    #region --- Methods ---

    private void HitKey(Gesture g)
    {
      if (g.Hold)
      {
        foreach (Key k in g.Command) inputSimulator.Keyboard.KeyDown((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
      }
      else
      {
        foreach (Key k in g.Command) inputSimulator.Keyboard.KeyDown((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
        foreach (Key k in g.Command) inputSimulator.Keyboard.KeyUp((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
      }
    }

    private void ReleaseKey(Gesture g)
    {
      foreach (Key k in g.Command) inputSimulator.Keyboard.KeyUp((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
    }

    private void HighlightGestureOnList(Gesture g)
    {
      g.IsHit = true;
    }
    private void DeHighlightGestureOnList(Gesture g)
    {
      g.IsHit = false;
    }

    private void HighlightFrames_Visualizer(Gesture g, List<GestureFrame> fs)
    {
      Model3DGroup modelGroup = new Model3DGroup();
      // Create material
      SolidColorBrush materialBrush = new SolidColorBrush()
      {
        Color = Colors.White,
        Opacity = 0.3
      };
      EmissiveMaterial material = new EmissiveMaterial(materialBrush);
      foreach (GestureFrame f in fs)
      {
        foreach (GestureFrameCell fc in f.FrontCells.Where(fc => fc.IsHotspot == true))
        {
          int fcIndex = Array.IndexOf(f.FrontCells, fc);
          foreach (GestureFrameCell sc in f.SideCells.Where(
              sc => sc.IsHotspot == true && (int)(Array.IndexOf(f.SideCells, sc) / 20) == (int)(fcIndex / 20)))
          {
            // Init mesh
            MeshBuilder meshBuilder = new MeshBuilder(false, false);
            // Make cube and add to mesh
            double y = (fc.LeftCM + fc.RightCM) / 2;
            double z = (fc.TopCM + fc.BottomCM) / 2;
            double x = (sc.LeftCM + sc.RightCM) / 2;
            Point3D cubeCenter = new Point3D(x, y, z);
            meshBuilder.AddBox(cubeCenter, 15, 15, 15);
            // Create and freeze mesh
            var mesh = meshBuilder.ToMesh(true);
            // Create models
            modelGroup.Children.Add(new GeometryModel3D(mesh, material));
          }
        }
      }
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = modelGroup;
      CollisionHighlights_Front.Children[GestureCollection.IndexOf(g)] = modelGroup;
      CollisionHighlights_Side.Children[GestureCollection.IndexOf(g)] = modelGroup;
    }

    private void DeHighlightFrames_Visualizer(Gesture g)
    {
      // De-highlight frame on grids
      Model3DGroup modelGroup_3D = (Model3DGroup)HotspotCellsModelVisual3D_Hit_Visualizer.Content;
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = new Model3DGroup();
      CollisionHighlights_Front.Children[GestureCollection.IndexOf(g)] = new Model3DGroup();
      CollisionHighlights_Side.Children[GestureCollection.IndexOf(g)] = new Model3DGroup();
    }

    private void HighLightFrames_Editor(Gesture g, List<GestureFrame> fs)
    {
      Model3DGroup modelGroup = new Model3DGroup();
      // Create material
      SolidColorBrush materialBrush = new SolidColorBrush()
      {
        Color = Colors.White,
        Opacity = 0.3
      };
      EmissiveMaterial material = new EmissiveMaterial(materialBrush);
      foreach (GestureFrame f in fs)
      {
        foreach (GestureFrameCell fc in f.FrontCells.Where(fc => fc.IsHotspot == true))
        {
          int fcIndex = Array.IndexOf(f.FrontCells, fc);
          foreach (GestureFrameCell sc in f.SideCells.Where(
              sc => sc.IsHotspot == true && (int)(Array.IndexOf(f.SideCells, sc) / 20) == (int)(fcIndex / 20)))
          {
            // Init mesh
            MeshBuilder meshBuilder = new MeshBuilder(false, false);
            // Make cube and add to mesh
            double y = (fc.LeftCM + fc.RightCM) / 2;
            double z = (fc.TopCM + fc.BottomCM) / 2;
            double x = (sc.LeftCM + sc.RightCM) / 2;
            Point3D cubeCenter = new Point3D(x, y, z);
            meshBuilder.AddBox(cubeCenter, 15, 15, 15);
            // Create and freeze mesh
            var mesh = meshBuilder.ToMesh(true);
            // Create models
            modelGroup.Children.Add(new GeometryModel3D(mesh, material));
          }
        }
      }
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = modelGroup;
    }

    private void DeHighlightFrames_Editor(Gesture g)
    {
      // De-highlight frame on grids
      Model3DGroup modelGroup_3D = (Model3DGroup)HotspotCellsModelVisual3D_Hit_Visualizer.Content;
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = new Model3DGroup();
    }

    #endregion

    #region --- Events ---

    private void SkeletonFrameReady_Draw3D_Editor(object sender, SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Editor, e);
    }

    private void SkeletonFrameReady_Draw3D_Front_Editor(object sender, SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Editor_FrontViewPort, e);
    }

    private void SkeletonFrameReady_Draw3D_Side_Editor(object sender, SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Editor_SideViewPort, e);
    }

    private void SkeletonFrameReady_ToggleBackground_Editor(object sender, SkeletonFrameReadyEventArgs e)
    {
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        if (skeletonFrame == null)
        {
          FrontViewImage.Visibility = System.Windows.Visibility.Visible;
          SideViewImage.Visibility = System.Windows.Visibility.Visible;
          return;
        }
        Skeleton[] skeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
        skeletonFrame.CopySkeletonDataTo(skeletons);
        if (skeletons == null)
        {
          FrontViewImage.Visibility = System.Windows.Visibility.Visible;
          SideViewImage.Visibility = System.Windows.Visibility.Visible;
          return;
        }
        Skeleton skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
        if (skeleton == null)
        {
          FrontViewImage.Visibility = System.Windows.Visibility.Visible;
          SideViewImage.Visibility = System.Windows.Visibility.Visible;
          return;
        }
        else
        {
          FrontViewImage.Visibility = System.Windows.Visibility.Hidden;
          SideViewImage.Visibility = System.Windows.Visibility.Hidden;
          return;
        }
      }
    }

    private void SkeletonFrameReady_Draw3D_Visualizer(object sender, SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Visualizer, e);
    }

    private void Draw3dSkeleton(ModelVisual3D modelVisual3D, SkeletonFrameReadyEventArgs e) //TODO: refactor into more methods
    {
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        // Get tracked skeleton data from stream, return if no data
        if (skeletonFrame == null)
        {
          modelVisual3D.Content = null;
          return;
        }
        Skeleton[] skeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
        skeletonFrame.CopySkeletonDataTo(skeletons);
        if (skeletons == null)
        {
          modelVisual3D.Content = null;
          return;
        }
        Skeleton skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
        if (skeleton == null)
        {
          modelVisual3D.Content = null;
          return;
        }

        // Waist coordinates will be origin
        SkeletonPoint centroid = skeleton.Joints[JointType.HipCenter].Position;

        // Init 3D stuff
        Model3DGroup modelGroup = new Model3DGroup();
        MeshBuilder meshBuilder = new MeshBuilder(false, false);

        // Init dict to tidy up code
        Dictionary<JointType, Point3D> jd = new Dictionary<JointType, Point3D>();

        // Add joints to mesh while populating the dict
        foreach (Joint j in skeleton.Joints)
        {
          // Helix3D has a different coordinate system
          int y = (int)((j.Position.X - centroid.X) * 100);
          int z = (int)((j.Position.Y - centroid.Y) * 100);
          int x = (int)((centroid.Z - j.Position.Z) * 100);
          jd[j.JointType] = new Point3D() { X = x, Y = y, Z = z };
          meshBuilder.AddSphere(new Point3D(x, y, z), 5);
        }

        // Add bones to mesh
        meshBuilder.AddCylinder(jd[JointType.Head], jd[JointType.ShoulderCenter], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ShoulderCenter], jd[JointType.Spine], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.Spine], jd[JointType.HipCenter], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.HipCenter], jd[JointType.HipLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.HipLeft], jd[JointType.KneeLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.KneeLeft], jd[JointType.AnkleLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.AnkleLeft], jd[JointType.FootLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.HipCenter], jd[JointType.HipRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.HipRight], jd[JointType.KneeRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.KneeRight], jd[JointType.AnkleRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.AnkleRight], jd[JointType.FootRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ShoulderCenter], jd[JointType.ShoulderLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ShoulderLeft], jd[JointType.ElbowLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ElbowLeft], jd[JointType.WristLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.WristLeft], jd[JointType.HandLeft], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ShoulderCenter], jd[JointType.ShoulderRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ShoulderRight], jd[JointType.ElbowRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.ElbowRight], jd[JointType.WristRight], 6, 10);
        meshBuilder.AddCylinder(jd[JointType.WristRight], jd[JointType.HandRight], 6, 10);

        // Create and freeze mesh
        var mesh = meshBuilder.ToMesh(true);

        // Create material
        Material blueMaterial = MaterialHelper.CreateMaterial(Colors.SteelBlue);

        // Create model
        modelGroup.Children.Add(new GeometryModel3D(mesh, blueMaterial));

        // Draw
        modelVisual3D.Content = modelGroup;
      }
    }

    private void SkeletonFrameReady_Detect_Editor(object sender, SkeletonFrameReadyEventArgs e) //TODO: refactor into more methods
    {
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        // Get data from stream, kill if no data
        if (skeletonFrame == null) return;
        Skeleton[] skeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
        skeletonFrame.CopySkeletonDataTo(skeletons);
        if (skeletonFrame == null || skeletons == null) return;
        // Mind only the tracked
        Skeleton skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
        if (skeleton == null) return;
        // Init stuff
        SkeletonPoint centroid = skeleton.Joints[JointType.HipCenter].Position;

        // Get the data
        // Check each gesture in collection
        for (int gi = 0; gi < GestureCollection.Count; gi++)
        {
          Gesture g = GestureCollection[gi];
          // Get coordinates of tracked joint
          // SkeletonPoint structure contains X. Y, Z values in meters, relative to the sensor
          // We'll convert that to a Point3D that contains values in cm, relative to the centroid        
          SkeletonPoint trackedJoint = skeleton.Joints[g.Joint].Position;
          int jX = (int)((trackedJoint.X - centroid.X) * 100);
          int jY = (int)((trackedJoint.Y - centroid.Y) * 100);
          int jZ = (int)((centroid.Z - trackedJoint.Z) * 100);
          // Check each frame in gesture for collision
          CollisionStates[gi][0] = CollisionStates[gi][1];
          CollisionStates[gi][1] = JointCollisionStates.OutThere;
          for (int fi = 0; fi < g.Frames.Count; fi++)
          {
            GestureFrame f = g.Frames[fi];
            // Check for collision
            foreach (GestureFrameCell fc in f.FrontCells.Where(c => c.IsHotspot))
            {
              if (jX <= fc.RightCM &&
                  jX >= fc.LeftCM &&
                  jY <= fc.TopCM &&
                  jY >= fc.BottomCM)
              {
                foreach (GestureFrameCell sc in f.SideCells.Where(c => c.IsHotspot))
                {
                  if (jZ <= sc.RightCM &&
                      jZ >= sc.LeftCM &&
                      jY <= sc.TopCM &&
                      jY >= sc.BottomCM)
                  {
                    // Record collision
                    CollisionTimes[gi][fi] = DateTime.Now;
                    CollisionStates[gi][1] = JointCollisionStates.InHotspot;
                    if (fi + 1 == g.Frames.Count) CollisionStates[gi][1] = JointCollisionStates.InUltimateHotspot;
                  }
                }
              }
            }
          }

          // Handle 1-frame gestures
          // List highlight and 3D highlights
          if (g.Frames.Count == 1)
          {
            if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              HighLightFrames_Editor(g, new List<GestureFrame>() { g.Frames[0] });
              HighlightGestureOnList(g);
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && CollisionStates[gi][1] == JointCollisionStates.OutThere)
            {
              DeHighlightFrames_Editor(g);
              DeHighlightGestureOnList(g);
            }
          }

          // Handle multi-frame gestures
          else
          {
            // List highlight
            if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && !(CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot))
              DeHighlightGestureOnList(g);
            else if (!(CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot) && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              for (int i = 1; i < CollisionTimes[gi].Count; i++)
              {
                if (CollisionTimes[gi][i] - CollisionTimes[gi][i - 1] > CollisionTimeout) break;
                if (i + 1 == CollisionTimes[gi].Count) HighlightGestureOnList(g);
              }
            }

            // 3D highlights
            List<GestureFrame> FramesToHighlight = new List<GestureFrame>();
            if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.OutThere)
              DeHighlightFrames_Editor(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InHotspot)
            {
              if (CollisionTimes[gi].IndexOf(CollisionTimes[gi].Max()) == 0) HighLightFrames_Editor(g, new List<GestureFrame>() { g.Frames[0] });
              else DeHighlightFrames_Editor(g);
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
              DeHighlightFrames_Editor(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.OutThere)
              DeHighlightFrames_Editor(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.InHotspot)
              if (CollisionTimes[gi].IndexOf(CollisionTimes[gi].Max()) == 0) HighLightFrames_Editor(g, new List<GestureFrame>() { g.Frames[0] });
              else
              {
                FramesToHighlight = new List<GestureFrame>();
                FramesToHighlight.Add(g.Frames[0]);
                for (int i = 1; i < CollisionTimes[gi].Count; i++)
                {
                  TimeSpan ts = CollisionTimes[gi][i] - CollisionTimes[gi][i - 1];
                  if (ts.Ticks < 0) break;
                  else if (ts > CollisionTimeout)
                  {
                    DeHighlightFrames_Editor(g);
                    FramesToHighlight = new List<GestureFrame>();
                    break;
                  }
                  else FramesToHighlight.Add(g.Frames[i]);
                }
                HighLightFrames_Editor(g, FramesToHighlight);
              }
            else if (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              FramesToHighlight = new List<GestureFrame>();
              for (int i = 0; i < CollisionTimes[gi].Count - 1; i++)
              {
                TimeSpan ts = CollisionTimes[gi][i + 1] - CollisionTimes[gi][i];
                if (CollisionTimes[gi][i] > CollisionTimes[gi][i + 1] || ts > CollisionTimeout)
                {
                  FramesToHighlight = new List<GestureFrame>();
                  break;
                }
                FramesToHighlight.Add(g.Frames[i]);
                if (!FramesToHighlight.Contains(g.Frames.Last())) FramesToHighlight.Add(g.Frames.Last());
              }
              HighLightFrames_Editor(g, FramesToHighlight);
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && !(CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot))
              DeHighlightFrames_Editor(g);
          }
        }
      } // using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
    } //private void kinect_SkeletonFrameReady_Detect(object sender, SkeletonFrameReadyEventArgs e)

    //TODO: compare this with previous method, if very similar have both Editor and Visualizer implement an interface that gets passed to a 3d utility method that both of these call with Editor and Visualizer as param respectively
    private void SkeletonFrameReady_Detect_Visualizer(object sender, SkeletonFrameReadyEventArgs e)
    {
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        // Get data from stream, kill if no data
        if (skeletonFrame == null) return;
        Skeleton[] skeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
        skeletonFrame.CopySkeletonDataTo(skeletons);
        if (skeletonFrame == null || skeletons == null) return;
        // Mind only the tracked
        Skeleton skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
        if (skeleton == null) return;
        // Init stuff
        SkeletonPoint centroid = skeleton.Joints[JointType.HipCenter].Position;

        // Get the data
        // Check each gesture in collection
        for (int gi = 0; gi < GestureCollection.Count; gi++)
        {
          Gesture g = GestureCollection[gi];
          // Get coordinates of tracked joint
          // SkeletonPoint structure contains X. Y, Z values in meters, relative to the sensor
          // We'll convert that to a Point3D that contains values in cm, relative to the centroid        
          SkeletonPoint trackedJoint = skeleton.Joints[g.Joint].Position;
          int jX = (int)((trackedJoint.X - centroid.X) * 100);
          int jY = (int)((trackedJoint.Y - centroid.Y) * 100);
          int jZ = (int)((centroid.Z - trackedJoint.Z) * 100);
          // Check each frame in gesture for collision
          CollisionStates[gi][0] = CollisionStates[gi][1];
          CollisionStates[gi][1] = JointCollisionStates.OutThere;
          for (int fi = 0; fi < g.Frames.Count; fi++)
          {
            GestureFrame f = g.Frames[fi];
            // Check for collision
            foreach (GestureFrameCell fc in f.FrontCells.Where(c => c.IsHotspot))
            {
              if (jX <= fc.RightCM &&
                  jX >= fc.LeftCM &&
                  jY <= fc.TopCM &&
                  jY >= fc.BottomCM)
              {
                foreach (GestureFrameCell sc in f.SideCells.Where(c => c.IsHotspot))
                {
                  if (jZ <= sc.RightCM &&
                      jZ >= sc.LeftCM &&
                      jY <= sc.TopCM &&
                      jY >= sc.BottomCM)
                  {
                    // Record collision
                    CollisionTimes[gi][fi] = DateTime.Now;
                    if (fi + 1 == g.Frames.Count) CollisionStates[gi][1] = JointCollisionStates.InUltimateHotspot;
                    else CollisionStates[gi][1] = JointCollisionStates.InHotspot;
                  }
                }
              }
            }
          }

          // Handle 1-frame gestures
          // Keyboard emulation, list highlight and 3D highlights
          if (g.Frames.Count == 1)
          {
            if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              HitKey(g);
              HighlightGestureOnList(g);
              HighlightFrames_Visualizer(g, new List<GestureFrame>() { g.Frames[0] });
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && CollisionStates[gi][1] == JointCollisionStates.OutThere)
            {
              ReleaseKey(g);
              DeHighlightGestureOnList(g);
              DeHighlightFrames_Visualizer(g);
            }
          }

          // Handle multi-frame gestures
          else
          {
            // Keyboard emulation and list highlight
            if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && !(CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot))
            {
              ReleaseKey(g);
              DeHighlightGestureOnList(g);
            }
            else if (!(CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot) && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              for (int i = 1; i < CollisionTimes[gi].Count; i++)
              {
                if (CollisionTimes[gi][i] - CollisionTimes[gi][i - 1] > CollisionTimeout) break;
                if (i + 1 == CollisionTimes[gi].Count)
                {
                  HitKey(g);
                  HighlightGestureOnList(g);
                }
              }
            }

            // 3D highlights
            List<GestureFrame> FramesToHighlight = new List<GestureFrame>();
            if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.OutThere)
              DeHighlightFrames_Visualizer(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InHotspot)
            {
              if (CollisionTimes[gi].IndexOf(CollisionTimes[gi].Max()) == 0) HighlightFrames_Visualizer(g, new List<GestureFrame>() { g.Frames[0] });
              else DeHighlightFrames_Visualizer(g);
            }
            else if ((CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot) ||
                     (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.OutThere))
              DeHighlightFrames_Visualizer(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.InHotspot)
              if (CollisionTimes[gi].IndexOf(CollisionTimes[gi].Max()) == 0) HighlightFrames_Visualizer(g, new List<GestureFrame>() { g.Frames[0] });
              else
              {
                FramesToHighlight = new List<GestureFrame>();
                FramesToHighlight.Add(g.Frames[0]);
                for (int i = 1; i < CollisionTimes[gi].Count; i++)
                {
                  TimeSpan ts = CollisionTimes[gi][i] - CollisionTimes[gi][i - 1];
                  if (ts.Ticks < 0) break;
                  else if (ts > CollisionTimeout)
                  {
                    DeHighlightFrames_Visualizer(g);
                    FramesToHighlight = new List<GestureFrame>();
                    break;
                  }
                  else FramesToHighlight.Add(g.Frames[i]);
                }
                HighlightFrames_Visualizer(g, FramesToHighlight);
              }
            else if (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              FramesToHighlight = new List<GestureFrame>();
              for (int i = 0; i < CollisionTimes[gi].Count - 1; i++)
              {
                TimeSpan ts = CollisionTimes[gi][i + 1] - CollisionTimes[gi][i];
                if (CollisionTimes[gi][i] > CollisionTimes[gi][i + 1] || ts > CollisionTimeout)
                {
                  FramesToHighlight = new List<GestureFrame>();
                  break;
                }
                FramesToHighlight.Add(g.Frames[i]);
                if (!FramesToHighlight.Contains(g.Frames.Last())) FramesToHighlight.Add(g.Frames.Last());
              }
              HighlightFrames_Visualizer(g, FramesToHighlight);
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && !(CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot))
              DeHighlightFrames_Visualizer(g);
          }
        }
      } // using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
    } //private void kinect_SkeletonFrameReady_Detect(object sender, SkeletonFrameReadyEventArgs e)

    #endregion

  }
}
