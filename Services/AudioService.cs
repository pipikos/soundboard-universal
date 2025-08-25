using System;
using System.Collections.Generic;
using LibVLCSharp.Shared;

namespace Soundboard.Universal.Services
{
    public class AudioService : IDisposable
    {
        private readonly LibVLC _libvlc;
        private readonly List<MediaPlayer> _active = new();
        private readonly Dictionary<int, MediaPlayer> _exclusive = new();

        public AudioService()
        {
            Core.Initialize(); // requires VLC installed on the system
            _libvlc = new LibVLC();
        }

        public void PlayOverlap(string filePath, int index, float volume01)
        {
            var media = new Media(_libvlc, new Uri(filePath));
            var mp = new MediaPlayer(media);
            mp.Volume = (int)Math.Clamp(volume01 * 100f, 0, 100);
            lock (_active) _active.Add(mp);
            mp.EndReached += (_, __) => Cleanup(mp);
            mp.EncounteredError += (_, __) => Cleanup(mp);
            mp.Play();
        }

        public void PlayCut(string filePath, int index, float volume01)
        {
            // stop existing on this pad
            if (_exclusive.TryGetValue(index, out var existing))
            {
                try { existing.Stop(); existing.Dispose(); } catch { }
                _exclusive.Remove(index);
            }
            var media = new Media(_libvlc, new Uri(filePath));
            var mp = new MediaPlayer(media);
            mp.Volume = (int)Math.Clamp(volume01 * 100f, 0, 100);
            _exclusive[index] = mp;
            mp.EndReached += (_, __) =>
            {
                Cleanup(mp);
                _exclusive.Remove(index);
            };
            mp.EncounteredError += (_, __) =>
            {
                Cleanup(mp);
                _exclusive.Remove(index);
            };
            mp.Play();
        }

        public void StopAll()
        {
            lock (_active)
            {
                foreach (var p in _active.ToArray())
                    Cleanup(p);
                _active.Clear();
            }
            foreach (var kv in _exclusive)
                try { kv.Value.Stop(); kv.Value.Dispose(); } catch { }
            _exclusive.Clear();
        }

        private void Cleanup(MediaPlayer mp)
        {
            try { mp.Stop(); } catch { }
            try { mp.Dispose(); } catch { }
            lock (_active) _active.Remove(mp);
        }

        public void Dispose()
        {
            StopAll();
            _libvlc?.Dispose();
        }
    }
}
