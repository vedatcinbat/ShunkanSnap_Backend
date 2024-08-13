namespace MusicTrackService.Requests;

public class CreateMusicTrackRequest
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public string Genre { get; set; }
    public string Duration { get; set; }
}