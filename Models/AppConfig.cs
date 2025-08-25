using System.Collections.Generic;

namespace Soundboard.Universal.Models
{
    public class AppConfig
    {
        public int GridRows { get; set; } = 4;
        public int GridCols { get; set; } = 4;
        public int ButtonFontSize { get; set; } = 14;
        public int ButtonPadding { get; set; } = 6;
        public List<Pad> Pads { get; set; } = new();

        public static AppConfig CreateDefault()
        {
            return new AppConfig
            {
                GridRows = 4,
                GridCols = 4,
                ButtonFontSize = 14,
                ButtonPadding = 6,
                Pads = new List<Pad>
                {
                    new Pad { Label = "Intro", FilePath = null, Mode = "Cut", Volume = 1.0f, Color = "#3b82f6" },
                    new Pad { Label = "Clap", FilePath = null, Mode = "Overlap", Volume = 0.9f, Color = "#16a34a" },
                }
            };
        }
    }
}
