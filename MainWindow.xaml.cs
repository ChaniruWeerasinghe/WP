using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace WallpaperSwitcher
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("crash.txt", ex.ToString());
                throw;
            }

            this.Title = "Wallpaper Switcher";
            WallpaperGridView.ItemsSource = ViewModel.Images;
            
            SetWindowFloatingAndPosition();
        }

        private void SetWindowFloatingAndPosition()
        {
            try
            {
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

                this.ExtendsContentIntoTitleBar = true;
                // Allow native title bar buttons to render normally

                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsMaximizable = false;
                    presenter.IsMinimizable = true; // Set to true so minimize works
                    presenter.IsResizable = false;
                    presenter.IsAlwaysOnTop = true;
                }

                DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                var workArea = displayArea.WorkArea;

                int windowWidth = 320;
                int windowHeight = 480;
                int marginX = 24; 
                int marginY = 24; 

                int x = workArea.X + workArea.Width - windowWidth - marginX;
                int y = workArea.Y + marginY;

                appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, windowWidth, windowHeight));
            }
            catch (Exception ex)
            {
                ShowToast($"Setup Error: {ex.Message}", true);
            }
        }

        private void OnImageClicked(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ImageModel image)
            {
                bool success = WallpaperService.ChangeWallpaper(image.FilePath);
                if (success)
                {
                    ShowToast("Wallpaper Updated", false);
                }
                else
                {
                    ShowToast("Failed to update wallpaper", true);
                }
            }
        }

        private void OnPreviousClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviousPage();
        }

        private void OnNextClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.NextPage();
        }

        private async void ShowToast(string message, bool isError)
        {
            ToastText.Text = message;
            ToastNotification.Background = isError 
                ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkRed) 
                : (Microsoft.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["SystemAccentColor"];
            
            // Fade in
            Storyboard fadeIn = new Storyboard();
            DoubleAnimation animIn = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(300) };
            Storyboard.SetTarget(animIn, ToastNotification);
            Storyboard.SetTargetProperty(animIn, "Opacity");
            fadeIn.Children.Add(animIn);
            fadeIn.Begin();

            await Task.Delay(2500);

            // Fade out
            Storyboard fadeOut = new Storyboard();
            DoubleAnimation animOut = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300) };
            Storyboard.SetTarget(animOut, ToastNotification);
            Storyboard.SetTargetProperty(animOut, "Opacity");
            fadeOut.Children.Add(animOut);
            fadeOut.Begin();
        }
    }
}
