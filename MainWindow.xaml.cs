using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Grid grid = new Grid();
        }

        public void SetImageToBackground()
        {
            BG.Source = null;
            BG.Source = ImageMethods.GetWindowBoundsImage(this);
            BG.Opacity = 0.6;
        }

        internal void KeyPressed(Key key, bool? isActive = null) 
        {
            switch (key)
            {
                case Key.Escape:
                    CloseAllWindows();
                    break;
                default:
                    break;
            }
        }

        private void CloseAllWindows()
        {
            WindowCollection allWindows = Application.Current.Windows;
            foreach (Window window in allWindows)
            {
                window.Close();
            }

            GC.Collect();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            KeyPressed(e.Key);
        }

        private void CancelItemClick(object sender, RoutedEventArgs e)
        {
            CloseAllWindows();
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e) 
        {
            TopButtonStack.Visibility = Visibility.Visible;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e) 
        {
            TopButtonStack.Visibility = Visibility.Collapsed;
        }

        private async void FreezeScreen() 
        {
            BackgroundBrush.Opacity = 0;
            await Task.Delay(100);
            SetImageToBackground();
        } 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            TopButtonStack.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            WindowState = WindowState.Maximized;
            FullWindow.Rect = new System.Windows.Rect(0, 0, Width, Height);
            KeyDown += HandleKeyDown;

            FreezeScreen();
            if (IsMouseOver)
            {
                TopButtonStack.Visibility = Visibility.Visible;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            BG.Source = null;
            TopButtonStack.Visibility = Visibility.Collapsed;
            CancelButton.Click -= CancelItemClick;
            KeyDown -= HandleKeyDown;
        }

        private void RegionClickCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            TopButtonStack.Visibility = Visibility.Collapsed;
        }

        private void RegionClickCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            TopButtonStack.Visibility = Visibility.Visible;
        }

    }
}