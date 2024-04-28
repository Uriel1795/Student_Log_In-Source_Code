using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace loginproto
{
    public partial class CustomFileExplorerWindow : Window
    {
        public CustomFileExplorerWindow()
        {
            InitializeComponent();

            LoadFiles(@"R:\");  // Load initial directory
        }

        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (fileListView.SelectedItem is FileItem selectedItem)
            {
                var fullPath = selectedItem.FullPath;

                if (Directory.Exists(fullPath))
                {
                    LoadFiles(fullPath);  // Navigate to the selected directory
                }
                else if (File.Exists(fullPath))
                {
                    // Open the file with its default application
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fullPath,
                        UseShellExecute = true
                    });
                }
            }
        }

        private void LoadFiles(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Directory not found.");
                return;
            }

            fileListView.Items.Clear();  // Clear previous items

            var files = Directory.GetFiles(directoryPath).OrderBy(f => f);
            var directories = Directory.GetDirectories(directoryPath).OrderBy(d => d);

            foreach (var dir in directories)
            {
                fileListView.Items.Add(new FileItem
                {
                    DisplayName = $"{Path.GetFileName(dir)}",
                    FullPath = dir,
                    IsDirectory = true
                });
            }

            foreach (var file in files)
            {
                fileListView.Items.Add(new FileItem
                {
                    DisplayName = Path.GetFileName(file),
                    FullPath = file,
                    IsDirectory = false
                });
            }
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Calculate the number of items per row based on the window's width
            double windowWidth = e.NewSize.Width;
            double itemWidth = 50;  // Example width of each icon (including margin)

            // Adjust the WrapPanel's item count to avoid overflow

            if (wrapPanel != null)
            {
                wrapPanel.Width = windowWidth;  // Set the WrapPanel's width to match the window
            }

            // Additional logic can be added here to adjust item layout based on the new size
        }
    }

    public class FileItem
    {
        public string DisplayName { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
    }
}
