# Wallpaper Switcher

A beautiful, modern Windows desktop application built with WinUI 3 and .NET 8 to easily manage and switch your wallpapers. 

## Features

- **Modern Aesthetics**: Built with WinUI 3, featuring a sleek user interface and a transparent acrylic glass backdrop that blends beautifully with your desktop environment.
- **Pin to Corner (Mini-Widget)**: Click the pin icon to collapse the application into a tiny, unobtrusive widget that stays on top of other windows for quick and easy access.
- **Quick Switch**: Instantly change your desktop wallpaper with a single click on any image.
- **Custom Folder Selection**: Easily configure the folder where your wallpapers are stored.
- **Standalone Execution**: Completely portable. No installation required.

## How to Download and Run

For new users, running the application is straightforward. You do not need to build from source.

1. Navigate to the **Releases** section on the right side of this GitHub repository.
2. Download the latest `WallpaperSwitcher-vX.X.X-win-x64-SingleFile.zip` from the release assets.
3. Extract the downloaded ZIP file to a folder of your choice.
4. Double-click on `WallpaperSwitcher.exe` to run the application.

## Requirements

- Windows 10 (Version 1809 or later) or Windows 11
- No extra dependencies are required as the application is self-contained.

## Development

If you wish to build the application from source:

1. Clone the repository.
2. Open `WallpaperSwitcher.csproj` in Visual Studio 2022.
3. Ensure the run profile is set to **WallpaperSwitcher (Unpackaged)**.
4. Press F5 to build and run.

Alternatively, via the .NET CLI:
```powershell
dotnet run --project WallpaperSwitcher.csproj
```

## License

This project is provided as-is. Feel free to fork and modify it for your own personal use.
