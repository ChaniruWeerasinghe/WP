using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace WallpaperSwitcher
{
    public class MainViewModel
    {
        public ObservableCollection<ImageModel> Images { get; set; }
        private List<string> _allImages;
        private int _currentPage = 0;
        private const int PageSize = 6;

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
                string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                string wallpapersPath = Path.Combine(picturesPath, "Wallpapers");
                if (!Directory.Exists(wallpapersPath))
                {
                    wallpapersPath = picturesPath;
                }

                if (Directory.Exists(wallpapersPath))
                {
                    _allImages = Directory.EnumerateFiles(wallpapersPath, "*.*", SearchOption.TopDirectoryOnly)
                                         .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                                                     s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                     s.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                         .ToList();

                    UpdatePagedImages();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading images: {ex.Message}");
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
            var pageFiles = _allImages.Skip(_currentPage * PageSize).Take(PageSize);
            foreach (var file in pageFiles)
            {
                Images.Add(new ImageModel(file));
            }
        }
    }
}
