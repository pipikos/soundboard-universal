using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Soundboard.Universal.Models;
using Soundboard.Universal.Services;
using Soundboard.Universal.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Timers;
using TimersTimer = System.Timers.Timer;

namespace Soundboard.Universal
{
    public partial class MainWindow : Window
    {
        private readonly string _configPath;
        private AppConfig _config = AppConfig.CreateDefault();
        private FileSystemWatcher? _watcher;
        private TimersTimer? _debounce;
        private bool _suppressWatcher = false;

        private readonly AudioService _audio = new();

        public MainWindow()
        {
            InitializeComponent();

            _configPath = ConfigService.ResolveConfigPath();
            LoadConfig();
            BuildGrid();
            SetupWatcher();

            Title = $"Soundboard Universal — {_configPath}";

            BtnStopAll.Click += (_, __) => _audio.StopAll();
            BtnReload.Click += (_, __) => { LoadConfig(); BuildGrid(); };
            BtnApplyGrid.Click += (_, __) =>
            {
                if (int.TryParse(TxtRows.Text, out var r)) _config.GridRows = Math.Max(1, r);
                if (int.TryParse(TxtCols.Text, out var c)) _config.GridCols = Math.Max(1, c);
                if (int.TryParse(TxtFont.Text, out var f)) _config.ButtonFontSize = Math.Clamp(f, 8, 48);
                if (int.TryParse(TxtPadding.Text, out var p)) _config.ButtonPadding = Math.Max(0, p);
                SaveConfig();
                BuildGrid();
            };

            Closed += (_, __) =>
            {
                _audio.StopAll();
                _audio.Dispose();
                _watcher?.Dispose();
                _debounce?.Dispose();
            };
        }

        private void LoadConfig()
        {
            _config = ConfigService.Load(_configPath);

            TxtRows.Text = _config.GridRows.ToString();
            TxtCols.Text = _config.GridCols.ToString();
            TxtFont.Text = _config.ButtonFontSize.ToString();
            TxtPadding.Text = _config.ButtonPadding.ToString();
        }

        private void SaveConfig()
        {
            _suppressWatcher = true;
            ConfigService.Save(_configPath, _config);
            // small delay before re-enabling watcher
            DispatcherTimer.RunOnce(() => _suppressWatcher = false, TimeSpan.FromMilliseconds(150));
        }

        private void SetupWatcher()
        {
            _watcher?.Dispose();
            _debounce?.Dispose();

            var dir = Path.GetDirectoryName(_configPath)!;
            var file = Path.GetFileName(_configPath);
            _watcher = new FileSystemWatcher(dir, file)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes
            };
            _watcher.Changed += (_, __) =>
            {
                if (_suppressWatcher) return;
                _debounce?.Stop();
                _debounce?.Start();
            };
            _watcher.EnableRaisingEvents = true;

            _debounce = new TimersTimer(200) { AutoReset = false };
            _debounce.Elapsed += (_, __) =>
            {
                if (_suppressWatcher) return;
                Dispatcher.UIThread.Post(() =>
                {
                    LoadConfig();
                    BuildGrid();
                });
            };
        }

        private void BuildGrid()
        {
            PadsHost.Children.Clear();
            PadsHost.ColumnDefinitions.Clear();
            PadsHost.RowDefinitions.Clear();

            var rows = Math.Max(1, _config.GridRows);
            var cols = Math.Max(1, _config.GridCols);

            for (int c = 0; c < cols; c++)
                PadsHost.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            for (int r = 0; r < rows; r++)
                PadsHost.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            // fill pads list to grid size
            int total = rows * cols;
            var pads = (_config.Pads ?? new List<Pad>()).Take(total).ToList();
            while (pads.Count < total) pads.Add(new Pad());
            _config.Pads = pads;

            int idx = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var pad = pads[idx];
                    var btn = CreatePadButton(pad, idx);
                    Grid.SetRow(btn, r);
                    Grid.SetColumn(btn, c);
                    PadsHost.Children.Add(btn);
                    idx++;
                }
            }
        }

        private Control CreatePadButton(Pad pad, int index)
        {
            var btn = new Button
            {
                Content = PadLabel(pad, index),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                Padding = new Thickness(_config.ButtonPadding),
                FontSize = _config.ButtonFontSize
            };
            // background color
            try
            {
                var color = Color.Parse(string.IsNullOrWhiteSpace(pad.Color) ? "#2d2d2d" : pad.Color!);
                btn.Background = new SolidColorBrush(color);
                btn.Foreground = Brushes.White;
            }
            catch
            {
                btn.Background = new SolidColorBrush(Color.Parse("#2d2d2d"));
                btn.Foreground = Brushes.White;
            }

            btn.Tag = (pad, index);
            btn.Click += (_, __) => PlayPad(pad, index);

            // right-click opens editor window
            btn.PointerReleased += async (s, e) =>
            {
                if (e.InitialPressMouseButton == MouseButton.Right)
                {
                    await OpenPadEditor(pad, (Button)s! , index);
                }
            };

            return btn;
        }

        private string PadLabel(Pad pad, int index)
        {
            var name = string.IsNullOrWhiteSpace(pad.Label) ? $"Pad {index + 1}" : pad.Label!;
            var mode = string.IsNullOrWhiteSpace(pad.Mode) ? "" : $"\n<{pad.Mode}>";
            return name + mode;
        }

        private async System.Threading.Tasks.Task OpenPadEditor(Pad pad, Button btn, int index)
        {
            var dlg = new Views.PadEditorWindow(pad);
            var res = await dlg.ShowDialog<bool?>(this);
            if (res == true)
            {
                SaveConfig();
                btn.Content = PadLabel(pad, index);
                btn.FontSize = _config.ButtonFontSize;
                btn.Padding = new Thickness(_config.ButtonPadding);
                try
                {
                    var color = Color.Parse(string.IsNullOrWhiteSpace(pad.Color) ? "#2d2d2d" : pad.Color!);
                    btn.Background = new SolidColorBrush(color);
                    btn.Foreground = Brushes.White;
                }
                catch { }
            }
        }

        private void PlayPad(Pad pad, int index)
        {
            if (string.IsNullOrWhiteSpace(pad.FilePath) || !File.Exists(pad.FilePath))
            {
                var mb = new Window
                {
                    Content = new TextBlock { Text = $"File not found:\n{pad.FilePath}", Margin = new Thickness(16) },
                    Width = 420, Height = 160, Title = "Missing file"
                };
                mb.Show(this);
                return;
            }

            var masterVol = (float)(VolumeSlider.Value / 100.0);
            var vol = Math.Clamp((pad.Volume <= 0 ? 1f : pad.Volume) * masterVol, 0f, 1f);

            if (string.Equals(pad.Mode, "Cut", StringComparison.OrdinalIgnoreCase))
                _audio.PlayCut(pad.FilePath!, index, vol);
            else
                _audio.PlayOverlap(pad.FilePath!, index, vol);
        }
    }
}
