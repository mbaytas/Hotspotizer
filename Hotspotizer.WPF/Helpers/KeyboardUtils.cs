﻿//Project: Hotspotizer (https://github.com/mbaytas/hotspotizer, https://github.com/birbilis/hotspotizer)
//File: KeyboardUtils.cs
//Version: 20150823

using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;
using Hotspotizer.Models;

namespace Hotspotizer.Helpers
{
  public static class KeyboardUtils
  {

    #region --- Fields ---

    private static InputSimulator inputSimulator = new InputSimulator(); // Init input simulator

    #endregion

    #region --- Methods ---

    public static void HitKey(Gesture g)
    {
      if (g.Hold)
        foreach (Key k in g.Command) inputSimulator.Keyboard.KeyDown((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
      else
      {
        foreach (Key k in g.Command) inputSimulator.Keyboard.KeyDown((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
        foreach (Key k in g.Command) inputSimulator.Keyboard.KeyUp((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
      }
    }

    public static void ReleaseKey(Gesture g)
    {
      foreach (Key k in g.Command) inputSimulator.Keyboard.KeyUp((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
    }

    #endregion

  }
}
