using BlazorWasm.MelodicJourneysOfCollin.Models;

namespace BlazorWasm.MelodicJourneysOfCollin.Services;

public sealed class PlayerStateService
{
    private readonly List<MusicInfoModel> _queue = new(MusicService.LocalPlaylist);

    public IReadOnlyList<MusicInfoModel> Queue => _queue;
    public MusicInfoModel? CurrentTrack { get; private set; }
    public int CurrentIndex { get; private set; } = -1;
    public bool IsPlaying { get; private set; }
    public double CurrentTime { get; private set; }
    public double Duration { get; private set; }
    public double Volume { get; private set; } = 0.72;
    public bool IsPlaylistOpen { get; private set; }
    public int SeekRequestToken { get; private set; }
    public double RequestedSeekTime { get; private set; }

    public event Action? Changed;

    public PlayerStateService()
    {
        if (_queue.Count > 0)
        {
            CurrentIndex = 0;
            CurrentTrack = _queue[0];
        }
    }

    public void PlayTrack(MusicInfoModel track, IEnumerable<MusicInfoModel>? queue = null)
    {
        if (queue is not null)
        {
            SetQueueInternal(queue);
        }
        else if (_queue.Count == 0)
        {
            SetQueueInternal(MusicService.LocalPlaylist);
        }

        var index = _queue.FindIndex(item => Matches(item, track));
        if (index < 0)
        {
            SetQueueInternal(MusicService.LocalPlaylist);
            index = _queue.FindIndex(item => Matches(item, track));
        }

        if (index >= 0)
        {
            PlayTrackAt(index);
            return;
        }

        CurrentTrack = track;
        CurrentIndex = -1;
        IsPlaying = true;
        CurrentTime = 0;
        Duration = 0;
        NotifyStateChanged();
    }

    public void PlayQueue(IEnumerable<MusicInfoModel> queue, int startIndex = 0)
    {
        SetQueueInternal(queue);
        if (_queue.Count == 0)
        {
            CurrentTrack = null;
            CurrentIndex = -1;
            IsPlaying = false;
            CurrentTime = 0;
            Duration = 0;
            NotifyStateChanged();
            return;
        }

        PlayTrackAt(Math.Clamp(startIndex, 0, _queue.Count - 1));
    }

    public void PlayTrackAt(int index)
    {
        if (index < 0 || index >= _queue.Count)
        {
            return;
        }

        CurrentIndex = index;
        CurrentTrack = _queue[index];
        IsPlaying = true;
        CurrentTime = 0;
        Duration = 0;
        NotifyStateChanged();
    }

    public void PlayNext()
    {
        if (_queue.Count == 0)
        {
            return;
        }

        var nextIndex = CurrentIndex >= 0 ? (CurrentIndex + 1) % _queue.Count : 0;
        PlayTrackAt(nextIndex);
    }

    public void PlayPrevious()
    {
        if (_queue.Count == 0)
        {
            return;
        }

        var previousIndex = CurrentIndex > 0 ? CurrentIndex - 1 : _queue.Count - 1;
        PlayTrackAt(previousIndex);
    }

    public void TogglePlayback()
    {
        if (CurrentTrack is null && _queue.Count > 0)
        {
            PlayTrackAt(Math.Max(CurrentIndex, 0));
            return;
        }

        SetPlaybackState(!IsPlaying);
    }

    public void SetPlaybackState(bool isPlaying)
    {
        IsPlaying = isPlaying;
        NotifyStateChanged();
    }

    public void SetTimeline(double currentTime, double duration)
    {
        CurrentTime = currentTime;
        Duration = duration;
        NotifyStateChanged();
    }

    public void SetVolume(double volume)
    {
        Volume = Math.Clamp(volume, 0, 1);
        NotifyStateChanged();
    }

    public void RequestSeek(double currentTime)
    {
        RequestedSeekTime = Math.Max(currentTime, 0);
        CurrentTime = RequestedSeekTime;
        SeekRequestToken++;
        NotifyStateChanged();
    }

    public void TogglePlaylist()
    {
        IsPlaylistOpen = !IsPlaylistOpen;
        NotifyStateChanged();
    }

    public void ClosePlaylist()
    {
        if (!IsPlaylistOpen)
        {
            return;
        }

        IsPlaylistOpen = false;
        NotifyStateChanged();
    }

    private void SetQueueInternal(IEnumerable<MusicInfoModel> queue)
    {
        _queue.Clear();
        foreach (var track in queue)
        {
            if (!_queue.Any(existing => Matches(existing, track)))
            {
                _queue.Add(track);
            }
        }
    }

    private static bool Matches(MusicInfoModel left, MusicInfoModel right)
        => string.Equals(left.Link, right.Link, StringComparison.OrdinalIgnoreCase);

    private void NotifyStateChanged() => Changed?.Invoke();
}
