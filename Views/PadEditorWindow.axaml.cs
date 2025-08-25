using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Soundboard.Universal.Models;
using System;
using System.Globalization;

namespace Soundboard.Universal.Views
{
    public partial class PadEditorWindow : Window
    {
        private readonly Pad _pad;

        public PadEditorWindow(Pad pad)
        {
            InitializeComponent();
            _pad = pad;

            // Load current pad values into UI
            TxtLabel.Text = _pad.Label ?? "";
            TxtFile.Text = _pad.FilePath ?? "";
            RbCut.IsChecked = string.Equals(_pad.Mode, "Cut", StringComparison.OrdinalIgnoreCase);
            RbOverlap.IsChecked = !RbCut.IsChecked.GetValueOrDefault();
            TxtVolume.Text = _pad.Volume.ToString(CultureInfo.InvariantCulture);
            TxtColor.Text = string.IsNullOrWhiteSpace(_pad.Color) ? "#2d2d2d" : _pad.Color;

            // Events
            BtnBrowse.Click += BtnBrowse_Click;   // <— όπως ζήτησες
            BtnOk.Click += BtnOk_Click;
            BtnCancel.Click += (_, __) => Close(false);
        }

        private async void BtnBrowse_Click(object? sender, RoutedEventArgs e)
        {
            var files = await this.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Select audio file",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Audio files")
                        {
                            Patterns = new[] { "*.wav", "*.mp3", "*.aiff", "*.flac", "*.m4a", "*.wma" }
                        },
                        FilePickerFileTypes.All
                    }
                });

            if (files != null && files.Count > 0)
            {
                var f = files[0];
                var path = f.TryGetLocalPath();
                if (!string.IsNullOrEmpty(path))
                {
                    TxtFile.Text = path!;
                }
                else
                {
                    await InfoBox("This storage provider does not expose a local file path. Please choose a local file.");
                }
            }
        }

        private void BtnOk_Click(object? sender, RoutedEventArgs e)
        {
            _pad.Label = string.IsNullOrWhiteSpace(TxtLabel.Text) ? null : TxtLabel.Text;
            _pad.FilePath = string.IsNullOrWhiteSpace(TxtFile.Text) ? null : TxtFile.Text;
            _pad.Mode = RbCut.IsChecked == true ? "Cut" : "Overlap";

            if (float.TryParse(TxtVolume.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var vol))
                _pad.Volume = Math.Clamp(vol, 0f, 1f);

            _pad.Color = string.IsNullOrWhiteSpace(TxtColor.Text) ? "#2d2d2d" : TxtColor.Text;

            Close(true);
        }

        private async System.Threading.Tasks.Task InfoBox(string message)
        {
            var win = new Window
            {
                Title = "Info",
                Width = 420,
                Height = 160,
                Content = new TextBlock { Text = message, Margin = new Thickness(16) }
            };
            await win.ShowDialog(this);
        }
    }
}
