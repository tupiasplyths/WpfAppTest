// using System;
// using System.Configuration;
// using System.Data;
// using System.Diagnostics;
// using System.IO;
// using System.Linq;
// using System.Reflection;
// using System.Threading;
using Dapplo.Windows.User32;
using System.Windows;
using WpfAppTest.Extensions;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // private static Mutex? _mutex = null;
        private const string AppName = "J2EOCRTranslator";
        // private static string _logPath = Path.Combine(
        //     Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        //     "WpfAppTest_log.txt");

        // protected override void OnStartup(StartupEventArgs e)
        // {
        //     // Log application start
        //     LogMessage($"Application starting. Process ID: {Environment.ProcessId}");

        //     // Get all running instances of this application
        //     var currentProcess = Process.GetCurrentProcess();
        //     var processes = Process.GetProcessesByName(currentProcess.ProcessName)
        //         .Where(p => p.Id != currentProcess.Id)
        //         .ToList();

        //     LogMessage($"Found {processes.Count} other instances running");

        //     // Check for existing instance using a named mutex
        //     try
        //     {
        //         _mutex = new Mutex(true, AppName, out bool createdNew);

        //         if (!createdNew)
        //         {
        //             // App is already running
        //             LogMessage("Application instance already running. Exiting this instance.");

        //             // Try to activate the existing window
        //             foreach (var process in processes)
        //             {
        //                 try
        //                 {
        //                     // Try to bring the existing window to the foreground
        //                     NativeMethods.SetForegroundWindow(process.MainWindowHandle);
        //                     LogMessage($"Activated existing window with handle {process.MainWindowHandle}");
        //                     break;
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     LogMessage($"Failed to activate window: {ex.Message}");
        //                 }
        //             }

        //             // Exit this instance
        //             Current.Shutdown();
        //             return;
        //         }

        //         LogMessage("Successfully acquired mutex, continuing startup");
        //     }
        //     catch (Exception ex)
        //     {
        //         LogMessage($"Mutex error: {ex.Message}");
        //     }

        //     // Continue with normal startup
        //     base.OnStartup(e);
        // }

        // protected override void OnExit(ExitEventArgs e)
        // {
        //     LogMessage($"Application exiting. Process ID: {Process.GetCurrentProcess().Id}");

        //     // Release the mutex when the application exits
        //     if (_mutex != null && _mutex.WaitOne(TimeSpan.Zero, true))
        //     {
        //         _mutex.ReleaseMutex();
        //         LogMessage("Released mutex");
        //     }

        //     base.OnExit(e);
        // }

        private void StartApplication(object sender, StartupEventArgs e)
        {
            MainWindow window = new();
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Width = 40;
            window.Height = 40;
            window.WindowState = WindowState.Normal;

            DisplayInfo screen = new();

            Point screenCenterPoint = screen.ScaledCenterPoint();

            window.Left = screenCenterPoint.X - (40 / 2);
            window.Top = screenCenterPoint.Y - (40 / 2);

            window.Show();
            window.Activate();
        }

        // private static void LogMessage(string message)
        // {
        //     try
        //     {
        //         File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}");
        //     }
        //     catch
        //     {
        //         // Ignore logging errors
        //     }
        // }
    }
}
