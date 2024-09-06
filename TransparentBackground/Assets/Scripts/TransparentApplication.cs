using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentApplication : MonoBehaviour
{
    [SerializeField]
    private RectTransform target;

    public struct Margins
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }

    [DllImport("User32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("User32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("User32.dll")]
    public static extern int BringWindowToTop(IntPtr hWnd);

    [DllImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("Dwmapi.dll")]
    public static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins margins);

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    private IntPtr hWnd;
    private const UInt32 SWP_NOSIZE = 0x0001;
    private const UInt32 SWP_NOMOVE = 0x0002;

    private const int GWL_EXSTYLE = -20;
    private uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSTPARENT = 0x00000020;

    private void Awake()
    {
        Application.runInBackground = true;

        hWnd = GetActiveWindow();

        Margins margins = new Margins { leftWidth = -1 };

        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        BringWindowToTop(hWnd);
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE);
    }

    private IEnumerator Start()
    {
        bool toggle = true;

        while (true)
        {
            target.position = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.T))
            {
                toggle = !toggle;

                BringWindowToTop(hWnd);
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE);

                if (toggle)
                {
                    SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
                }
                else
                {
                    SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSTPARENT);
                }
            }

            yield return null;
        }
    }
}
