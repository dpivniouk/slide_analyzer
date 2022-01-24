using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Minimize : MonoBehaviour
{
#if UNITY_EDITOR
#else
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
#endif

    public void MinimizeWindow()
    {
#if UNITY_EDITOR
#else
        ShowWindow(GetActiveWindow(), 2);
#endif
    }
}
