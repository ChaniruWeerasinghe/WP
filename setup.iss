[Setup]
AppName=WallpaperSwitcher
AppVersion=1.0.3
DefaultDirName={autopf}\WallpaperSwitcher
DefaultGroupName=WallpaperSwitcher
SetupIconFile=C:\Work - Chanii\Projects\WP\appicon.ico
OutputBaseFilename=WallpaperSwitcher_Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\WallpaperSwitcher.exe

[Files]
Source: "C:\Work - Chanii\Projects\WP\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\WallpaperSwitcher"; Filename: "{app}\WallpaperSwitcher.exe"
Name: "{autodesktop}\WallpaperSwitcher"; Filename: "{app}\WallpaperSwitcher.exe"
Name: "{userstartup}\WallpaperSwitcher"; Filename: "{app}\WallpaperSwitcher.exe"

[Run]
Filename: "{app}\WallpaperSwitcher.exe"; Description: "Launch WallpaperSwitcher"; Flags: nowait postinstall skipifsilent
