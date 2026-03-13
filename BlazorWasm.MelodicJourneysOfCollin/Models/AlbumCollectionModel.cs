namespace BlazorWasm.MelodicJourneysOfCollin.Models;

public sealed class AlbumCollectionModel
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Cover { get; init; } = string.Empty;
    public IReadOnlyList<MusicInfoModel> Tracks { get; init; } = Array.Empty<MusicInfoModel>();
}
