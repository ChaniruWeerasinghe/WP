using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WallpaperSwitcher
{
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const int GWL_EXSTYLE = -20;
        const int GWLP_HWNDPARENT = -8;
        const long WS_EX_APPWINDOW = 0x00040000L;
        const long WS_EX_TOOLWINDOW = 0x00000080L;
        
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;

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
            
            this.Activated += MainWindow_Activated;
            SetWindowFloatingAndPosition();
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            // Force it to the very bottom of the window stack (on top of desktop, behind all apps)
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        private void SetWindowFloatingAndPosition()
        {
            try
            {
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

                // Hide from Taskbar and Alt-Tab
                long style = (long)GetWindowLongPtr(hWnd, GWL_EXSTYLE);
                SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr((style & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW));

                // Bind ownership to the Desktop (Progman) to prevent Win+D from hiding the widget
                IntPtr progman = FindWindow("Progman", null);
                if (progman != IntPtr.Zero)
                {
                    SetWindowLongPtr(hWnd, GWLP_HWNDPARENT, progman);
                }

                WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

                this.ExtendsContentIntoTitleBar = true;
                // Allow native title bar buttons to render normally

                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsMaximizable = false;
                    presenter.IsMinimizable = false; 
                    presenter.IsResizable = false;
                    presenter.IsAlwaysOnTop = false;
                    presenter.SetBorderAndTitleBar(true, false); // Completely removes native title bar
                }

                DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                var workArea = displayArea.WorkArea;

                int windowWidth = 520;
                int windowHeight = 440;
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

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool _isPinned = false;
        private const int NormalWidth = 520;
        private const int NormalHeight = 440;
        private const int PinnedWidth = 140;
        private const int PinnedHeight = 62; 

        private void OnPinClicked(object sender, RoutedEventArgs e)
        {
            _isPinned = !_isPinned;
            
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            
            DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;
            int marginX = 24; 
            int marginY = 24; 
            
            if (_isPinned)
            {
                PinIcon.Glyph = "\uE73F"; // Back to Window
                ToolTipService.SetToolTip(PinButton, "Expand");

                MainContentGrid.Visibility = Visibility.Collapsed;
                BottomAreaGrid.Visibility = Visibility.Collapsed;
                RootGrid.Padding = new Thickness(12, 10, 12, 10);
                
                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsAlwaysOnTop = true;
                }
                
                int x = workArea.X + workArea.Width - PinnedWidth - marginX;
                int y = workArea.Y + marginY;
                appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, PinnedWidth, PinnedHeight));
            }
            else
            {
                PinIcon.Glyph = "\uE718"; // Pin
                ToolTipService.SetToolTip(PinButton, "Pin to Corner");

                MainContentGrid.Visibility = Visibility.Visible;
                BottomAreaGrid.Visibility = Visibility.Visible;
                RootGrid.Padding = new Thickness(16, 12, 16, 8);
                
                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsAlwaysOnTop = false;
                }
                
                int x = workArea.X + workArea.Width - NormalWidth - marginX;
                int y = workArea.Y + marginY;
                appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, NormalWidth, NormalHeight));
            }
        }

        private async void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowHandle);

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                ViewModel.ChangeFolder(folder.Path);
                ShowToast("Folder updated!", false);
            }
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
