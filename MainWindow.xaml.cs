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
            //grid.Children.Add(vancas);
            //grid.Children.Add(clip);
            //Content = grid;
        }

        public void SetImageToBackground()
        {
            BG.Source = null;
            BG.Source = ImageMethods.GetWindowBoundsImage(this);
            BG.Opacity = 0.2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(vancas);
                // drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(-left, top), new Point(width - left, height - top)));
            }
            //renderTargetBitmap.Render(drawingVisual);

            // Save the image to a file
            BitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (var stream = System.IO.File.Create("./output/captured_image.png"))
            {
                encoder.Save(stream);
            }
        }

        // private async void FreezeScreen() 
        // {
        //     BackgroundBrush.Opacity = 0;
        // } 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            TopButtonStack.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            WindowState = WindowState.Maximized;
            FullWindow.Rect = new System.Windows.Rect(0, 0, Width, Height);

            if (IsMouseOver)
            {
                TopButtonStack.Visibility = Visibility.Visible;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            TopButtonStack.Visibility = Visibility.Collapsed;
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