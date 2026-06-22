using System;
using System.Runtime.InteropServices;

namespace WallpaperSwitcher
{
    public static class WallpaperService
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public static bool ChangeWallpaper(string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath) || !System.IO.File.Exists(imagePath))
                    return false;

                int result = SystemParametersInfo(
                    SPI_SETDESKWALLPAPER, 
                    0, 
                    imagePath, 
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                    
                return result != 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set wallpaper: {ex.Message}");
                return false;
            }
        }
    }
}
