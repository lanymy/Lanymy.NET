#if !NETSTANDARD

using System;
using System.Collections.Generic;
using System.Text;
using Lanymy.Common.Helpers;


namespace Lanymy.Common.Instruments.Helpers
{



    public class ConsoleHelper
    {

        public static void CenterConsole()
        {
            IntPtr hWin = Win32Helper.GetConsoleWindow();
            Win32Helper.GetWindowRect(hWin, out Win32Helper.RECT rc);

            int x = (Win32Helper.GetSystemMetrics(Win32Helper.SM_CXSCREEN) - (rc.right - rc.left)) / 2;
            int y = (Win32Helper.GetSystemMetrics(Win32Helper.SM_CYSCREEN) - (rc.bottom - rc.top)) / 2;

            Win32Helper.MoveWindow(hWin, x, y, rc.right - rc.left, rc.bottom - rc.top, true);

        }

    }

}

#endif