namespace MelodicJourney.CLI.Models
{
    public class MusicReleasePaginationModel
    {
        public List<MusicInfoModel> musicList { get; set; } = new List<MusicInfoModel>();
        public int totalPage { get; set; }
    }
}
