using System.Text.Json;
using BlazorWasm.MelodicJourneysOfCollin.Models;
using Microsoft.JSInterop;

namespace BlazorWasm.MelodicJourneysOfCollin.Services;

public sealed class LibraryStateService
{
    private const string StorageKey = "collin-favorites";
    private readonly IJSRuntime _jsRuntime;
    private readonly HashSet<string> _favoriteLinks = new(StringComparer.OrdinalIgnoreCase);

    public bool IsInitialized { get; private set; }
    public IReadOnlyList<MusicInfoModel> Tracks => MusicService.LocalPlaylist;
    public IReadOnlyList<AlbumCollectionModel> Albums => MusicService.Albums;

    public event Action? Changed;

    public LibraryStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        var saved = await _jsRuntime.InvokeAsync<string[]>("libraryStore.getFavorites", StorageKey);
        _favoriteLinks.Clear();

        foreach (var link in saved)
        {
            if (!string.IsNullOrWhiteSpace(link))
            {
                _favoriteLinks.Add(link);
            }
        }

        IsInitialized = true;
        Changed?.Invoke();
    }

    public bool IsFavorite(MusicInfoModel? track)
        => track?.Link is { Length: > 0 } link && _favoriteLinks.Contains(link);

    public IReadOnlyList<MusicInfoModel> GetFavorites()
        => Tracks.Where(IsFavorite).ToList();

    public AlbumCollectionModel? GetAlbum(string? albumId)
        => Albums.FirstOrDefault(album => string.Equals(album.Id, albumId, StringComparison.OrdinalIgnoreCase));

    public async Task ToggleFavoriteAsync(MusicInfoModel track)
    {
        if (string.IsNullOrWhiteSpace(track.Link))
        {
            return;
        }

        if (_favoriteLinks.Contains(track.Link))
        {
            _favoriteLinks.Remove(track.Link);
        }
        else
        {
            _favoriteLinks.Add(track.Link);
        }

        await _jsRuntime.InvokeVoidAsync("libraryStore.setFavorites", StorageKey, _favoriteLinks.ToArray());
        Changed?.Invoke();
    }
}
