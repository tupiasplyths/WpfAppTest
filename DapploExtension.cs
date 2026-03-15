using Dapplo.Windows.User32;
using System.Windows;

namespace WpfAppTest.Extensions;

public static class ExtensionMethods
{
    public static System.Windows.Point ScaledCenterPoint (this DisplayInfo displayInfo)
    {
        Rect displayRect = displayInfo.Bounds;
        NativeMethods.GetScaleFactorForMonitor(displayInfo.MonitorHandle, out uint scaleFactor);
        double scaleFraction = scaleFactor/100.0;
        System.Windows.Point rawCenter = displayRect.CenterPoint();
        System.Windows.Point displayCenter = new System.Windows.Point(rawCenter.X * scaleFraction, rawCenter.Y * scaleFraction);
        return displayCenter;
    }

    public static Rect ScaledBounds(this DisplayInfo displayInfo)
    {
        Rect displayRect = displayInfo.Bounds;
        NativeMethods.GetScaleFactorForMonitor(displayInfo.MonitorHandle, out uint scaleFactor);
        double scaleFraction = scaleFactor / 100.0;

        Rect scaleBounds = new  (displayRect.X / scaleFraction, displayRect.Y / scaleFraction, displayRect.Width / scaleFraction, displayRect.Height / scaleFraction);

        return scaleBounds;
    }
}

