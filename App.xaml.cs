using System;
using System.Diagnostics;
using System.Threading;
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
        private static Mutex? _mutex = null;
        private const string AppName = "J2EOCRTranslator";
        // private static string _logPath = Path.Combine(
        //     Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        //     "WpfAppTest_log.txt");

        protected override void OnStartup(StartupEventArgs e)
        {
            // Check for existing instance using a named mutex
            bool createdNew;
            try
            {
                _mutex = new Mutex(true, AppName, out createdNew);

                if (!createdNew)
                {
                    // App is already running - activate existing window and exit this instance
                    Console.WriteLine("Application instance already running. Exiting this instance.");

                    // Try to bring the existing window to the foreground
                    var currentProcess = Process.GetCurrentProcess();
                    var processes = Process.GetProcessesByName(currentProcess.ProcessName)
                        .Where(p => p.Id != currentProcess.Id)
                        .ToList();

                    foreach (var process in processes)
                    {
                        try
                        {
                            if (process.MainWindowHandle != IntPtr.Zero)
                            {
                                NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                                Console.WriteLine($"Activated existing window with handle {process.MainWindowHandle}");
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to activate window: {ex.Message}");
                        }
                    }

                    // Exit this instance
                    Current.Shutdown();
                    return;
                }

                Console.WriteLine("Successfully acquired mutex, continuing startup");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mutex error: {ex.Message}");
            }

            // Continue with normal startup
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Console.WriteLine($"Application exiting. Process ID: {Process.GetCurrentProcess().Id}");

            // Release the mutex when the application exits
            if (_mutex != null)
            {
                try
                {
                    if (_mutex.WaitOne(TimeSpan.Zero, true))
                    {
                        _mutex.ReleaseMutex();
                        Console.WriteLine("Released mutex");
                    }
                    _mutex.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error releasing mutex: {ex.Message}");
                }
            }

            base.OnExit(e);
        }

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

    }
}
