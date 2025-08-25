using System;
using System.IO;
using System.Text.Json;
using Soundboard.Universal.Models;

namespace Soundboard.Universal.Services
{
    public static class ConfigService
    {
        public static string ResolveConfigPath()
        {
            var exeDir = AppContext.BaseDirectory;
            return Path.Combine(exeDir, "config.json");
        }

        public static AppConfig Load(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    var cfg = AppConfig.CreateDefault();
                    Save(path, cfg);
                    return cfg;
                }
                var json = File.ReadAllText(path);
                var cfg2 = JsonSerializer.Deserialize<AppConfig>(json) ?? AppConfig.CreateDefault();
                return cfg2;
            }
            catch
            {
                return AppConfig.CreateDefault();
            }
        }

        public static void Save(string path, AppConfig cfg)
        {
            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
