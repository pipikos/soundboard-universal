# Soundboard Universal (Avalonia + LibVLCSharp)

Cross-platform soundboard (Windows / Linux / macOS) in .NET 8.

## Requirements
- .NET 8 SDK
- VLC installed (for libvlc native libraries)

## Run
```bash
dotnet restore
dotnet run

Here you go — a thorough, production-ready **README.md** for the cross-platform project (Avalonia + LibVLCSharp). It’s written for both users and developers and focuses on clear, step-by-step installation on Windows, Linux, and macOS.

---

# Soundboard Universal

A fast, cross-platform soundboard for **Windows / Linux / macOS** built with **.NET 8**, **Avalonia UI**, and **LibVLCSharp**.

* Grid of pads (set **rows/cols**, **font size**, **padding**)
* Per-pad settings dialog (right-click a pad): **Label**, **Audio file**, **Mode (Cut/Overlap)**, **Per-pad volume**, **Color**
* Master volume & “Stop All”
* Config stored in **`config.json`** (auto-saved, auto-reloaded with debounce)
* Uses **VLC** under the hood for broad audio codec support

> ✅ This app routes audio to your system’s **default output device**.
> To send audio into VoiceMeeter on Windows, set *VoiceMeeter Input* as your **default playback device** (or pick it in OS sound settings).

---

## Table of Contents

1. [System Requirements](#system-requirements)
2. [Download & Install (Prebuilt ZIPs)](#download--install-prebuilt-zips)
3. [First Run & Basic Usage](#first-run--basic-usage)
4. [Adding Sounds](#adding-sounds)
5. [Configuration (`config.json`)](#configuration-configjson)
6. [Building From Source](#building-from-source)
7. [Publishing Portable Binaries](#publishing-portable-binaries)
8. [Troubleshooting](#troubleshooting)
9. [Uninstall](#uninstall)
10. [Roadmap / Known Limitations](#roadmap--known-limitations)
11. [License & Credits](#license--credits)

---

## System Requirements

* **OS:**

  * Windows 10/11 x64
  * Linux x64 (glibc; tested on Debian/Ubuntu/Fedora/Arch)
  * macOS 12+ (Intel x64 build provided; Apple Silicon can run via Rosetta or rebuild)
* **Runtime:**

  * Prebuilt “self-contained” ZIPs **include** .NET — no runtime to install.
  * If you build “framework-dependent”, install **.NET 8 Runtime/SDK**.
* **VLC:**

  * **Must be installed** on the target machine so LibVLC can load native libraries.
  * Windows/macOS: install from the official VLC website.
  * Linux: `sudo apt install vlc` (Debian/Ubuntu), `sudo dnf install vlc` (Fedora), `sudo pacman -S vlc` (Arch).

> Why VLC? LibVLCSharp uses LibVLC for playback, giving excellent format support (wav/mp3/flac/aiff/m4a/wma, etc.).

---

## Download & Install (Prebuilt ZIPs)

1. Go to **Releases** on this repository and download the ZIP for your OS/RID:

   * `Soundboard.Universal-vX.Y.Z-win-x64.zip`
   * `Soundboard.Universal-vX.Y.Z-linux-x64.zip`
   * `Soundboard.Universal-vX.Y.Z-osx-x64.zip`
2. **Install VLC** on the machine (if not already installed).
3. **Extract** the ZIP to any folder you like (e.g., `C:\Apps\SoundboardUniversal` or `~/apps/soundboard-universal`).
4. Run the app:

   * **Windows:** double-click `Soundboard.Universal.exe`
   * **Linux:** `chmod +x Soundboard.Universal`; then `./Soundboard.Universal`
   * **macOS:** double-click the app (see Gatekeeper note in [Troubleshooting](#troubleshooting)).

### Notes

* The app stores/reads **`config.json` in the same folder** as the executable. Keep the file next to the app.
* On Windows SmartScreen or macOS Gatekeeper you may need to **Allow/Run anyway** (see [Troubleshooting](#troubleshooting)).

---

## First Run & Basic Usage

* **Top bar controls**

  * **Volume** slider: master gain (0–100%)
  * **Stop All**: stop every currently playing sound
  * **Rows / Cols**: change grid dimensions
  * **Font / Pad**: adjust button font size and padding
  * **Apply Grid**: rebuild the grid with the current layout
  * **Reload Config**: manually reload `config.json` (auto-reload also runs on file changes)
* **Pad buttons**

  * **Left-click**: play the pad
  * **Right-click**: open **Pad Settings** dialog:

    * **Text (Label)**
    * **Audio File** (file picker)
    * **Mode**:

      * **Cut** = stop previous instance of this pad before playing
      * **Overlap** = multiple instances can play simultaneously
    * **Volume (0..1)** per pad
    * **Color** (#RRGGBB)

Changes are **saved immediately** to `config.json`. The app **auto-reloads** the config with a short debounce; the UI updates on the fly.

---

## Adding Sounds

1. Right-click a pad → **Pad Settings**
2. Click **…** to choose an audio file (wav/mp3/flac/aiff/m4a/wma)
3. Optional: set **Mode**, **Volume**, **Label**, **Color**
4. **OK** to save

> For consistent file paths across systems, keep your audio files in a known folder (e.g., a subfolder inside the app directory) and avoid network mounts unless needed.

---

## Configuration (`config.json`)

The app writes/reads `config.json` next to the executable. Example:

```json
{
  "GridRows": 4,
  "GridCols": 4,
  "ButtonFontSize": 14,
  "ButtonPadding": 6,
  "Pads": [
    {
      "Label": "Intro",
      "FilePath": "C:\\\\Music\\\\intro.wav",
      "Mode": "Cut",
      "Volume": 1.0,
      "Color": "#3b82f6"
    },
    {
      "Label": "Clap",
      "FilePath": "/home/user/sounds/clap.mp3",
      "Mode": "Overlap",
      "Volume": 0.9,
      "Color": "#16a34a"
    }
  ]
}
```

* **`GridRows`/`GridCols`** — grid size (>= 1)
* **`ButtonFontSize`** — pad label font size (8–48 recommended)
* **`ButtonPadding`** — pad visual padding (>= 0)
* **`Pads[]`** — one entry per visible pad (the app will auto-extend with empty pads or truncate to the grid size)

> Edit by hand if you like, then use **Reload Config** — or just right-click pads and change settings via the dialog (auto-save is on).

---

## Building From Source

### 1) Prerequisites

* **.NET 8 SDK**

  * Windows:

    ```powershell
    winget install --id Microsoft.DotNet.SDK.8 -e
    ```
  * Linux/macOS: follow instructions at [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* **VLC** installed (see System Requirements above)
* **Git** (optional, for cloning)

  ```powershell
  # Windows
  winget install --id Git.Git -e
  ```

### 2) Clone & Run

```bash
git clone https://github.com/<your-username>/soundboard-universal.git
cd soundboard-universal
dotnet restore
dotnet run
```

---

## Publishing Portable Binaries

Produce self-contained builds (no .NET runtime needed on target machines):

### Windows (x64)

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### Linux (x64)

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

### macOS (x64)

```bash
dotnet publish -c Release -r osx-x64 --self-contained true
```

After publish, binaries are under:

```
bin/Release/net8.0/<RID>/publish/
```

Include **`config.json`** in that folder (the project is configured to copy it if it exists in the repo root).

### Multi-RID local packaging (optional)

A helper PowerShell script can build and zip multiple RIDs:

```powershell
powershell -ExecutionPolicy Bypass -File .\release-multi.ps1 -Version v0.2.0 -SelfContained
```

It produces:

```
Soundboard.Universal-v0.2.0-win-x64.zip
Soundboard.Universal-v0.2.0-linux-x64.zip
Soundboard.Universal-v0.2.0-osx-x64.zip
```

### GitHub Actions (optional)

A CI workflow can create Release zips automatically when you push a tag (e.g., `v0.2.0`). See `.github/workflows/universal-release.yml` in this repo.

---

## Troubleshooting

### “libvlc not found” / No sound

* Ensure **VLC is installed** on the machine (not just LibVLCSharp NuGet).
* On Linux, verify with `vlc --version`. If missing, install via your distro:

  * Debian/Ubuntu: `sudo apt update && sudo apt install vlc`
  * Fedora: `sudo dnf install vlc`
  * Arch: `sudo pacman -S vlc`
* Restart the app after installing VLC.

### Windows: route audio into VoiceMeeter

* Open **Sound Settings** → set **VoiceMeeter Input** as the **Default Output** device.
* Relaunch the app and play a pad — audio will arrive in VoiceMeeter.

### macOS: “App cannot be opened” (Gatekeeper)

* Right-click the app → **Open** → confirm.
* Or **System Settings → Privacy & Security** → “Allow Anyway”.

### SmartScreen (Windows)

* Click **More info** → **Run anyway**.
* This is normal for unsigned binaries.

### Self-contained vs Framework-dependent

* If you used `--self-contained false`, the target machine must have **.NET 8 Runtime** installed.

### Auto-reload loops / config edits

* The app watches `config.json` and reloads with a short debounce.
* If you experience rapid reloads, ensure your editor isn’t saving temp files into the same path.

### File chosen but pad won’t play

* Check that the **file path is valid** and accessible by the app’s user.
* Try playing the file in VLC itself to confirm codec support.

---

## Uninstall

The app is **portable**:

* Delete the folder where you extracted the ZIP.
* Delete the adjacent **`config.json`** (if you want to reset settings).

No registry keys, no system services.

---

## Roadmap / Known Limitations

* **Output device selection (per OS)** — currently uses the system default.
  *Planned:* expose LibVLC audio output modules (e.g., WASAPI/Pulse/CoreAudio) and device selection.
* **Global hotkeys (background)** — not implemented yet (pads trigger while app has focus).
  *Planned:* cross-platform global hotkeys (e.g., via SharpHook).
* **MIDI mapping** — not implemented yet.
  *Planned:* basic MIDI note→pad mapping.
* **Apple Silicon native build** — build RID `osx-arm64` or run x64 with Rosetta.

---

## License & Credits

* **License:** MIT (see `LICENSE`)
* **Credits:**

  * [Avalonia UI](https://avaloniaui.net/) — cross-platform UI framework
  * [LibVLCSharp](https://code.videolan.org/videolan/LibVLCSharp) & [VLC](https://www.videolan.org/vlc/) — audio playback
  * .NET 8 runtime

---

### Questions? Issues?

Please open an **Issue** in this repository with:

* OS and version
* App version (ZIP/release tag)
* How you installed VLC
* Steps to reproduce
* Any console output or screenshots

We’re happy to help!
