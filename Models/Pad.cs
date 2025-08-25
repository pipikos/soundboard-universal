namespace Soundboard.Universal.Models
{
    public class Pad
    {
        public string? Label { get; set; }
        public string? FilePath { get; set; }
        public string? Mode { get; set; } = "Overlap"; // "Cut" | "Overlap"
        public float Volume { get; set; } = 1.0f;      // 0..1
        public string? Color { get; set; } = "#2d2d2d";
        public string? Hotkey { get; set; }            // (προαιρετικό για μελλοντική χρήση)
    }
}
