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

        public ObservableCollection<ImageModel> Images { get; set; }
        private List<string> _allImages;
        private int _currentPage = 0;
        private const int PageSize = 6;
        private const string FolderSettingsKey = "CustomWallpaperFolder";

        public MainViewModel()
        {
            Images = new ObservableCollection<ImageModel>();
            _allImages = new List<string>();
            LoadImages();
        }

        private void LoadImages()
        {
            try
            {
                string wallpapersPath = null;
                
                // Check if user has saved a custom folder
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue(FolderSettingsKey, out object savedPath))
                {
                    wallpapersPath = savedPath as string;
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

                    _currentPage = 0;
                    UpdatePagedImages();
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
                ApplicationData.Current.LocalSettings.Values[FolderSettingsKey] = newPath;
                LoadImages();
            }
        }

        public void NextPage()
        {
            if ((_currentPage + 1) * PageSize < _allImages.Count)
            {
                _currentPage++;
                UpdatePagedImages();
            }
        }

        public void PreviousPage()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdatePagedImages();
            }
        }

        private void UpdatePagedImages()
        {
            Images.Clear();
            var paged = _allImages.Skip(_currentPage * PageSize).Take(PageSize);
            foreach (var img in paged)
            {
                Images.Add(new ImageModel(img));
            }
            
            EmptyStateVisibility = _allImages.Count == 0 ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}
