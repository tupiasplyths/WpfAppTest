using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double left = Canvas.GetLeft(clip);
            double top = Canvas.GetTop(clip);
            double width = clip.Width;
            double height = clip.Height;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(vancas);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(-left, top), new Point(width - left, height - top)));
            }
            renderTargetBitmap.Render(drawingVisual);

            // Save the image to a file
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (var stream = System.IO.File.Create("captured_image.png"))
            {
                encoder.Save(stream);
            }
        }

        private async void FreezeScreen() 
        {
            BackgroundBrush.Opacity = 0;
        } 
    }
}