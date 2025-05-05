using Dapplo.Windows.User32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using WpfAppTest.Utilities;
using WpfAppTest.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSelecting = false;
        private readonly MangaOCR OCR = new(); // Init OCR engine
        private Border selectBorder = new(); // Border for the selection rectangle
        private System.Windows.Point clickedPoint = new();
        private DisplayInfo? CurrentScreen { get; set; }
        private string? OCRText { get; set; }
        private string? TranslatedText { get; set; }
        private TextBox? editTextBox ;
        private bool isEditing = false;
        private bool useCustomOCR = false; // Track which OCR model to use

        public MainWindow()
        {
            InitializeComponent();
            Grid grid = new();
        }

        public void SetImageToBackground()
        {
            BG.Source = null;
            BG.Source = ImageMethods.GetWindowBoundsImage(this);
            BackgroundBrush.Opacity = 0.2;
        }

        internal void KeyPressed(Key key, bool? isActive = null)
        {
            switch (key)
            {
                case Key.Escape:
                    MinimizeWindow();
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
            // CloseAllWindows();
            Quit();
            return;
        }

        private void MinimizeWindow()
        {
            WindowState = WindowState.Minimized;
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            TopButtonStack.Visibility = Visibility.Visible;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            TopButtonStack.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the MouseDown event on the canvas, initiating a selection process.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs instance containing the event data.</param>
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            isSelecting = true;
            TopButtonStack.Visibility = Visibility.Collapsed;
            vancas.CaptureMouse();
            CursorClipper.ClipCursor(this);
            clickedPoint = e.GetPosition(this);
            selectBorder.Height = 2;
            selectBorder.Width = 2;
            translatedTextBlock.Text = "";

            try { vancas.Children.Remove(selectBorder); } catch (Exception) { }
            selectBorder.BorderThickness = new Thickness(2);
            System.Windows.Media.Color borderColor = System.Windows.Media.Color.FromArgb(255, 40, 118, 126);
            selectBorder.BorderBrush = new SolidColorBrush(borderColor);
            _ = vancas.Children.Add(selectBorder);
            Canvas.SetLeft(selectBorder, clickedPoint.X);
            Canvas.SetTop(selectBorder, clickedPoint.Y);

            ApplicationUtilities.GetMousePosition(out System.Windows.Point mousePoint);
            foreach (DisplayInfo? screen in DisplayInfo.AllDisplayInfos)
            {
                Rect bound = screen.ScaledBounds();
                if (bound.Contains(mousePoint)) CurrentScreen = screen;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isSelecting) return;

            isSelecting = false;
            CurrentScreen = null;
            CursorClipper.UnClipCursor();
            vancas.ReleaseMouseCapture();
            clippingGeometry.Rect = new Rect(new System.Windows.Point(0, 0), new System.Windows.Size(0, 0));

            System.Windows.Point currentPoint = e.GetPosition(this);
            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            currentPoint.X *= m.M11;
            currentPoint.Y *= m.M22;

            currentPoint.X = Math.Round(currentPoint.X);
            currentPoint.Y = Math.Round(currentPoint.Y);

            double xDimension = Canvas.GetLeft(selectBorder) * m.M11;
            double yDimension = Canvas.GetTop(selectBorder) * m.M22;

            Rectangle scaledRegion = new(
                (int)xDimension,
                (int)yDimension,
                (int)(selectBorder.Width * m.M11),
                (int)(selectBorder.Height * m.M22));

            Bitmap bmp = ImageMethods.GetRegionOfScreenAsBitmap(scaledRegion);
            string timeStamp = ApplicationUtilities.GetTimestamp(DateTime.Now);
            // string cwd = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            bool isSmallArea = scaledRegion.Width < 5 && scaledRegion.Height < 5;
            if (isSmallArea)
            {
                BackgroundBrush.Opacity = 0;
                return;
            }
            string outputFileName = $"./output/{timeStamp}.png";
            bmp.Save(outputFileName, ImageFormat.Png);
            string text = useCustomOCR ? OCR.GetTextFromCustomOCR(outputFileName) : OCR.GetTextFromOCR(outputFileName);
            OCRText = text;
            TranslatedText = Translate.GetTranslation(text);
            Console.WriteLine(text + "\n" + TranslatedText);

            // translatedTextBlock.Text = translatedText;
            UpdateTextBlock(TranslatedText, scaledRegion, xDimension, yDimension);
            // CloseAllWindows();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting) return;

            System.Windows.Point currentPoint = e.GetPosition(this);
            double left = Math.Min(clickedPoint.X, currentPoint.X);
            double top = Math.Min(clickedPoint.Y, currentPoint.Y);

            selectBorder.Height = Math.Max(clickedPoint.Y, currentPoint.Y) - top;
            selectBorder.Width = Math.Max(clickedPoint.X, currentPoint.X) - left;
            selectBorder.Height += 2;
            selectBorder.Width += 2;

            clippingGeometry.Rect = new Rect(
                new System.Windows.Point(left, top),
                new System.Windows.Size(selectBorder.Width - 2, selectBorder.Height - 2));

            Canvas.SetLeft(selectBorder, left - 1);
            Canvas.SetTop(selectBorder, top - 1);
        }

        private void UpdateTextBlock(string translateText, Rectangle region, double xDimension = 0, double yDimension = 0)
        {
            translatedTextBlock.Text = translateText;
            translatedTextBlock.Width = region.Width;
            translatedTextBlock.Height = region.Height;

            Canvas.SetLeft(translatedTextBlock, xDimension);
            Canvas.SetTop(translatedTextBlock, yDimension);
            translatedTextBlock.VerticalAlignment = VerticalAlignment.Center;
        }

        private void EditTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FinishEditing();
        }

        private void TranslatedTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && !isEditing)
            {
                isEditing = true;
                // UpdateTextBlock("", new Rectangle(0, 0, 0, 0), 0, 0);

                editTextBox = new TextBox
                {
                    Text = OCRText,
                    Width = translatedTextBlock.Width,
                    Height = translatedTextBlock.Height,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = translatedTextBlock.FontSize,
                    Background = translatedTextBlock.Background,
                };

                Canvas.SetLeft(editTextBox, Canvas.GetLeft(translatedTextBlock));
                Canvas.SetTop(editTextBox, Canvas.GetTop(translatedTextBlock) - editTextBox.Height - 5);


                vancas.Children.Add(editTextBox);
                editTextBox.Focus();
                editTextBox.SelectAll();
                editTextBox.LostFocus += EditTextBox_LostFocus;
                editTextBox.KeyDown += EditTextBox_KeyDown;
            }
            e.Handled = true;
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing)
            {
                isEditing = true;

                editTextBox = new TextBox
                {
                    Text = OCRText, // Use original OCR text for editing
                    Width = translatedTextBlock.ActualHeight, // Use ActualWidth for better sizing
                    Height = translatedTextBlock.ActualWidth, // Use ActualHeight
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = translatedTextBlock.FontSize,
                    Background = translatedTextBlock.Background,
                    TextWrapping = TextWrapping.Wrap, // Ensure wrapping matches
                    AcceptsReturn = true // Allow multi-line editing if needed
                };
                Canvas.SetLeft(editTextBox, Canvas.GetLeft(translatedTextBlock) - translatedTextBlock.Width/3);
                Canvas.SetTop(editTextBox, Canvas.GetTop(translatedTextBlock) - editTextBox.Height - 5);

                double editLeft = Canvas.GetLeft(translatedTextBlock);
                double editTop = Canvas.GetTop(translatedTextBlock);


                // Hide the TextBlock and add the TextBox
                translatedTextBlock.Visibility = Visibility.Collapsed;
                vancas.Children.Add(editTextBox);
                editTextBox.Focus();
                editTextBox.SelectAll();
                editTextBox.LostFocus += EditTextBox_LostFocus;
                editTextBox.KeyDown += EditTextBox_KeyDown;
                // Show and position the Finish Edit Button
                FinishEditButton.Visibility = Visibility.Visible;
                // Position button below the textbox
                Canvas.SetLeft(FinishEditButton, editLeft + editTextBox.Width);
                Canvas.SetTop(FinishEditButton, editTop + editTextBox.Height/3);
            }
        }

        private void FinishEditButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Finished Editing");
            FinishEditing();
        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                FinishEditing();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                CancelEditing();
                e.Handled = true;
            }
        }

        private void FinishEditing()
        {
            if (!isEditing) return;

            if (editTextBox?.Text == null) return;
            // OCRText = editTextBox.Text;
            TranslatedText = Translate.GetTranslation(editTextBox.Text);
            translatedTextBlock.Text = TranslatedText;

            translatedTextBlock.Visibility = Visibility.Visible;
            vancas.Children.Remove(editTextBox);
            editTextBox = null;
            isEditing = false;
        }

        private void CancelEditing()
        {
            if (!isEditing) return;

            translatedTextBlock.Visibility = Visibility.Visible;
            vancas.Children.Remove(editTextBox);
            FinishEditButton.Visibility = Visibility.Collapsed;
            editTextBox = null;
            isEditing = false;
        }

        private async void FreezeScreen()
        {
            BackgroundBrush.Opacity = 0;
            await Task.Delay(150);
            SetImageToBackground();
        }

        private void Unfreeze()
        {
            BackgroundBrush.Opacity = 0;
            BG.Source = null;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Unfreeze();
            UpdateTextBlock("", new Rectangle(0, 0, 0, 0), 0, 0);
            selectBorder.BorderThickness = new Thickness(0);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            FreezeScreen();
            if (!isEditing)
            {
                FinishEditButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Quit()
        {
            GC.Collect();
            MangaOCR.CleanUp();
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TopButtonStack.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            FullWindow.Rect = new Rect(0, 0, Width, Height);
            KeyDown += HandleKeyDown;
            SetImageToBackground();
            ModelToggleButton.ToolTip = "Using Manga OCR (Click to switch to Custom OCR)";

            if (IsMouseOver)
            {
                TopButtonStack.Visibility = Visibility.Visible;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            BG.Source = null;
            BG.UpdateLayout();
            CurrentScreen = null;
            Loaded -= Window_Loaded;
            Unloaded -= Window_Unloaded;
            KeyDown -= HandleKeyDown;
            TopButtonStack.Visibility = Visibility.Collapsed;
            CancelButton.Click -= CancelItemClick;
            vancas.MouseDown -= Canvas_MouseDown;
            vancas.MouseUp -= Canvas_MouseUp;
            vancas.MouseMove -= Canvas_MouseMove;
            vancas.MouseEnter -= Canvas_MouseEnter;
            vancas.MouseLeave -= Canvas_MouseLeave;

            if (editTextBox != null)
            {
                editTextBox.LostFocus -= EditTextBox_LostFocus;
                editTextBox.KeyDown -= EditTextBox_KeyDown;
            }

            // OCR.CleanUp();
            GC.Collect();
        }

        private void ModelToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            useCustomOCR = true;
            ModelToggleButton.ToolTip = "Using Custom OCR (Click to switch to Manga OCR)";
        }

        private void ModelToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            useCustomOCR = false;
            ModelToggleButton.ToolTip = "Using Manga OCR (Click to switch to Custom OCR)";
        }
    }
}