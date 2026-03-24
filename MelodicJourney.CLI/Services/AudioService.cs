using NAudio.Wave;
using MelodicJourney.CLI.Models;
using System.IO;

namespace MelodicJourney.CLI.Services;

public class AudioService : IDisposable
{
    private IWavePlayer? _outputDevice;
    private AudioFileReader? _audioFile;

    public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;
    public MusicInfoModel? CurrentTrack { get; private set; }

    public Task PlayAsync(MusicInfoModel track)
    {
        if (string.IsNullOrWhiteSpace(track.Link)) return Task.CompletedTask;

        var playbackPath = track.Link;
        if (!track.Link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            playbackPath = Path.GetFullPath(track.Link);
            if (!File.Exists(playbackPath))
            {
                throw new FileNotFoundException($"Music file not found: {track.Link}\nLooking at: {playbackPath}\n\n", playbackPath);
            }
        }

        try
        {
            Stop(); // Stop current playback if any

            _outputDevice = new WaveOutEvent();
            _audioFile = new AudioFileReader(playbackPath);
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();

            CurrentTrack = track;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to initialize NAudio playback: {ex.Message}", ex);
        }

        return Task.CompletedTask;
    }

    public Task PauseAsync()
    {
        _outputDevice?.Pause();
        return Task.CompletedTask;
    }

    public Task ResumeAsync()
    {
        _outputDevice?.Play();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        Stop();
        return Task.CompletedTask;
    }

    private void Stop()
    {
        _outputDevice?.Stop();
        _outputDevice?.Dispose();
        _outputDevice = null;

        _audioFile?.Dispose();
        _audioFile = null;

        CurrentTrack = null;
    }

    public Task SetVolumeAsync(byte volume)
    {
        if (_audioFile != null)
        {
            // NAudio volume is 0.0 to 1.0f
            _audioFile.Volume = volume / 100f;
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Stop();
    }
}

