using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace WallpaperSwitcher
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        private Microsoft.UI.Xaml.Visibility _emptyStateVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        public Microsoft.UI.Xaml.Visibility EmptyStateVisibility
        {
            get => _emptyStateVisibility;
            set
            {
                if (_emptyStateVisibility != value)
                {
                    _emptyStateVisibility = value;
                    OnPropertyChanged(nameof(EmptyStateVisibility));
                }
            }
        }

        private string _folderName = "Configure Folder";
        public string FolderName
        {
            get => _folderName;
            set
            {
                if (_folderName != value)
                {
                    _folderName = value;
                    OnPropertyChanged(nameof(FolderName));
                }
            }
        }

        private string _folderIcon = "\xE713"; // Settings gear by default
        public string FolderIcon
        {
            get => _folderIcon;
            set
            {
                if (_folderIcon != value)
                {
                    _folderIcon = value;
                    OnPropertyChanged(nameof(FolderIcon));
                }
            }
        }

        public ObservableCollection<ImageModel> Images { get; set; }
        private List<string> _allImages;
        private const string FolderSettingsKey = "CustomWallpaperFolder";

        public MainViewModel()
        {
            Images = new ObservableCollection<ImageModel>();
            _allImages = new List<string>();
            LoadImages();
        }

        private string GetSettingsFilePath()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(folder, "WallpaperSwitcherApp");
            Directory.CreateDirectory(appFolder);
            return Path.Combine(appFolder, "settings.txt");
        }

        private void LoadImages()
        {
            try
            {
                string wallpapersPath = null;
                string settingsFile = GetSettingsFilePath();
                
                // Check if user has saved a custom folder
                if (File.Exists(settingsFile))
                {
                    wallpapersPath = File.ReadAllText(settingsFile);
                }

                // Fallback to default
                if (string.IsNullOrEmpty(wallpapersPath) || !Directory.Exists(wallpapersPath))
                {
                    string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    wallpapersPath = Path.Combine(picturesPath, "Wallpapers");
                    if (!Directory.Exists(wallpapersPath))
                    {
                        wallpapersPath = picturesPath;
                    }
                }

                if (Directory.Exists(wallpapersPath))
                {
                    _allImages = Directory.EnumerateFiles(wallpapersPath, "*.*", SearchOption.TopDirectoryOnly)
                                         .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                                                     s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                     s.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                         .ToList();

                    Images.Clear();
                    foreach (var img in _allImages)
                    {
                        Images.Add(new ImageModel(img));
                    }
                    EmptyStateVisibility = _allImages.Count == 0 ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
                    
                    FolderName = new DirectoryInfo(wallpapersPath).Name;
                    FolderIcon = "\xED25"; // Folder icon
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading images: {ex.Message}");
            }
        }

        public void ChangeFolder(string newPath)
        {
            if (Directory.Exists(newPath))
            {
                try
                {
                    File.WriteAllText(GetSettingsFilePath(), newPath);
                }
                catch { } // Ignore save errors
                LoadImages();
            }
        }
    }
}
