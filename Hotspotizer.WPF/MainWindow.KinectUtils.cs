//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer)
//File: MainWindow.KinectUtils.cs
//Version: 20150823

using HelixToolkit.Wpf;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Hotspotizer.Models;
using Hotspotizer.Helpers;

namespace Hotspotizer
{
  public partial class MainWindow : Window
  {

    #region --- Fields ---

    // Kinect-related members
    private KinectSensor kinect = null;

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

    // Gesture recognition stuff
    enum JointCollisionStates { InHotspot, InUltimateHotspot, OutThere };
    List<JointCollisionStates[]> CollisionStates;
    List<List<DateTime>> CollisionTimes;
    TimeSpan CollisionTimeout = TimeSpan.FromMilliseconds(500);

    #endregion

    #region --- Methods ---

    public KinectSensor GetKinectSensor()
    {
      return KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
    }

    private void Set_ViewImages_Visibility(Visibility visibility)
    {
      FrontViewImage.Visibility = SideViewImage.Visibility = visibility;
    }

    #region SkeletonFrameReady

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
          Point3D center = new Point3D { X = x, Y = y, Z = z };
          jd[j.JointType] = center;
          meshBuilder.AddSphere(center, 5);
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

        var mesh = meshBuilder.ToMesh(true); // Create and freeze mesh
        Material blueMaterial = MaterialHelper.CreateMaterial(Colors.SteelBlue); // Create material
        modelGroup.Children.Add(new GeometryModel3D(mesh, blueMaterial)); // Create model

        // Draw
        modelVisual3D.Content = modelGroup;
      }
    }

    private void SkeletonFrameReady_Detect(SkeletonFrameReadyEventArgs e, HighlightFramesDelegate highlightFrames, DeHighlightFramesDelegate deHighlightFrames, bool keyboardEmulation) //TODO: refactor into smaller methods
    {
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        // Get data from stream
        if (skeletonFrame == null)
          return;
        Skeleton[] skeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
        skeletonFrame.CopySkeletonDataTo(skeletons);

        // Mind only the tracked, return if none
        Skeleton skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
        if (skeleton == null)
          return;
        
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
                    if (fi + 1 == g.Frames.Count)
                      CollisionStates[gi][1] = JointCollisionStates.InUltimateHotspot;
                    else
                      CollisionStates[gi][1] = JointCollisionStates.InHotspot;
                  }
                }
              }
            }
          }

          // Handle 1-frame gestures
          // Keyboard emulation, List highlight and 3D highlights
          if (g.Frames.Count == 1)
          {
            if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              if (keyboardEmulation)
                KeyboardUtils.HitKey(g);
              HighlightGestureOnList(g);
              highlightFrames(g, new List<GestureFrame>() { g.Frames[0] });
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && CollisionStates[gi][1] == JointCollisionStates.OutThere)
            {
              if (keyboardEmulation)
                KeyboardUtils.ReleaseKey(g);
              DeHighlightGestureOnList(g);
              deHighlightFrames(g);
            }
          }

          // Handle multi-frame gestures
          else
          {
            // Keyboard emulation and List highlight
            if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && !(CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot))
            {
              if (keyboardEmulation)
                KeyboardUtils.ReleaseKey(g);
              DeHighlightGestureOnList(g);
            }
            else if (!(CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot) && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot)
            {
              for (int i = 1; i < CollisionTimes[gi].Count; i++)
              {
                if (CollisionTimes[gi][i] - CollisionTimes[gi][i - 1] > CollisionTimeout) break;
                if (i + 1 == CollisionTimes[gi].Count)
                {
                  if (keyboardEmulation)
                    KeyboardUtils.HitKey(g);
                  HighlightGestureOnList(g);
                }
              }
            }

            // 3D highlights
            List<GestureFrame> FramesToHighlight = new List<GestureFrame>();
            if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.OutThere)
              deHighlightFrames(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InHotspot)
            {
              if (CollisionTimes[gi].IndexOf(CollisionTimes[gi].Max()) == 0)
                highlightFrames(g, new List<GestureFrame>() { g.Frames[0] });
              else
                deHighlightFrames(g);
            }
            else if ((CollisionStates[gi][0] == JointCollisionStates.OutThere && CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot) ||
                     (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.OutThere))
              deHighlightFrames(g);
            else if (CollisionStates[gi][0] == JointCollisionStates.InHotspot && CollisionStates[gi][1] == JointCollisionStates.InHotspot)
              if (CollisionTimes[gi].IndexOf(CollisionTimes[gi].Max()) == 0)
                highlightFrames(g, new List<GestureFrame>() { g.Frames[0] });
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
                    deHighlightFrames(g);
                    FramesToHighlight = new List<GestureFrame>();
                    break;
                  }
                  else FramesToHighlight.Add(g.Frames[i]);
                }
                highlightFrames(g, FramesToHighlight);
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
              highlightFrames(g, FramesToHighlight);
            }
            else if (CollisionStates[gi][0] == JointCollisionStates.InUltimateHotspot && !(CollisionStates[gi][1] == JointCollisionStates.InUltimateHotspot))
              deHighlightFrames(g);
          }
        }
      } // using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
    } // SkeletonFrameReady_Detect method

    #region Editor

    private void SkeletonFrameReady_Draw3D_Editor(SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Editor, e);
    }

    private void SkeletonFrameReady_Draw3D_Front_Editor(SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Editor_FrontViewPort, e);
    }

    private void SkeletonFrameReady_Draw3D_Side_Editor(SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Editor_SideViewPort, e);
    }

    private void SkeletonFrameReady_ToggleBackground_Editor(SkeletonFrameReadyEventArgs e)
    {
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        if (skeletonFrame != null)
        {
          Skeleton[] skeletons = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
          skeletonFrame.CopySkeletonDataTo(skeletons);

          Skeleton skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
          if (skeleton != null) //if there is a tracked skeleton
          {
            Set_ViewImages_Visibility(Visibility.Hidden); //hide the 2D view images (will show skeleton)
            return;
          }
        }
        Set_ViewImages_Visibility(Visibility.Visible); //if there is no skeletonFrame or tracked skeleton show the 2D view images
      }
    }

    private void SkeletonFrameReady_Detect_Editor(SkeletonFrameReadyEventArgs e)
    {
      SkeletonFrameReady_Detect(e, HighlightFrames_Editor, DeHighlightFrames_Editor, keyboardEmulation: false);
    }

    #endregion

    #region Visualizer

    private void SkeletonFrameReady_Draw3D_Visualizer(SkeletonFrameReadyEventArgs e)
    {
      Draw3dSkeleton(SkeletonModelVisual3D_Visualizer, e);
    }

    private void SkeletonFrameReady_Detect_Visualizer(SkeletonFrameReadyEventArgs e)
    {
      SkeletonFrameReady_Detect(e, HighlightFrames_Visualizer, DeHighlightFrames_Visualizer, keyboardEmulation: true);
    }

    #endregion

    #endregion

    #region Highlighting

    private delegate void HighlightFramesDelegate(Gesture g, List<GestureFrame> fs);
    private delegate void DeHighlightFramesDelegate(Gesture g);

    private Model3DGroup GetHighlightModel3D(Gesture g, List<GestureFrame> fs)
    {
      // Create material
      EmissiveMaterial material = new EmissiveMaterial(new SolidColorBrush() { Color = Colors.White, Opacity = 0.3 });

      Model3DGroup modelGroup = new Model3DGroup();
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
      return modelGroup;
    }

    #region Editor

     /// <summary>
    /// Highlight frames on editor grids
    /// </summary>
    /// <param name="g">Gesture</param>
    /// <param name="fs">Gesture Frames</param>
    private void HighlightFrames_Editor(Gesture g, List<GestureFrame> fs)
    {
      Model3DGroup modelGroup = GetHighlightModel3D(g, fs);
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = modelGroup;
    }

    /// <summary>
    /// De-highlight frames on editor grids
    /// </summary>
    /// <param name="g">Gesture</param>
    private void DeHighlightFrames_Editor(Gesture g)
    {
      //Model3DGroup modelGroup_3D = (Model3DGroup)HotspotCellsModelVisual3D_Hit_Visualizer.Content; //this seems to be unused
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = new Model3DGroup(); //TODO: maybe there is a bug here and wanted to use modelGroup_3D instead?
    }

    #endregion

    #region Visualizer

    /// <summary>
    /// Highlight frames on visualizer grids
    /// </summary>
    /// <param name="g">Gesture</param>
    /// <param name="fs">Gesture Frames</param>
    private void HighlightFrames_Visualizer(Gesture g, List<GestureFrame> fs)
    {
      Model3DGroup modelGroup = GetHighlightModel3D(g, fs);
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = modelGroup;
      CollisionHighlights_Front.Children[GestureCollection.IndexOf(g)] = modelGroup;
      CollisionHighlights_Side.Children[GestureCollection.IndexOf(g)] = modelGroup;
    }

    /// <summary>
    /// De-highlight frames on visualizer grids
    /// </summary>
    /// <param name="g">Gesture</param>
    private void DeHighlightFrames_Visualizer(Gesture g)
    {
      //Model3DGroup modelGroup_3D = (Model3DGroup)HotspotCellsModelVisual3D_Hit_Visualizer.Content; //this seems to be unused
      CollisionHighlights_3D.Children[GestureCollection.IndexOf(g)] = new Model3DGroup(); //TODO: maybe there is a bug here and wanted to use modelGroup_3D instead?
      CollisionHighlights_Front.Children[GestureCollection.IndexOf(g)] = new Model3DGroup(); //TODO: maybe reuse the "new Model3DGroup()" instead of creating 3 of those?
      CollisionHighlights_Side.Children[GestureCollection.IndexOf(g)] = new Model3DGroup();
    }

    #endregion

    #region List

    private void HighlightGestureOnList(Gesture g)
    {
      g.IsHit = true;
    }

    private void DeHighlightGestureOnList(Gesture g)
    {
      g.IsHit = false;
    }

    #endregion

    #endregion Highlighting

    #endregion Methods

  }
}
