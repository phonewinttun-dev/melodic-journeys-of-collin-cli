using NetCoreAudio;
using MelodicJourney.CLI.Models;
using System.IO;

namespace MelodicJourney.CLI.Services;

public sealed class AudioService
{
    private readonly Player _player = new();
    public bool IsPlaying { get; private set; }
    public MusicInfoModel? CurrentTrack { get; private set; }

    public async Task PlayAsync(MusicInfoModel track)
    {
        if (string.IsNullOrWhiteSpace(track.Link)) return;

        // handle local files and urls
        if (!track.Link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var fullPath = Path.GetFullPath(track.Link);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Music file not found: {track.Link}\nLooking at: {fullPath}\n\n", fullPath);
            }
        }

        await _player.Play(track.Link);
        CurrentTrack = track;
        IsPlaying = true;
    }

    public async Task PauseAsync()
    {
        await _player.Pause();
        IsPlaying = false;
    }

    public async Task ResumeAsync()
    {
        await _player.Resume();
        IsPlaying = true;
    }

    public async Task StopAsync()
    {
        await _player.Stop();
        IsPlaying = false;
        CurrentTrack = null;
    }

    public async Task SetVolumeAsync(byte volume)
    {
        await _player.SetVolume(volume);
    }
}
