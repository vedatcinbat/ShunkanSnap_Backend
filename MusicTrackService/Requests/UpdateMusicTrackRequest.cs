namespace MusicTrackService.Requests;

public class UpdateMusicTrackRequest
{
    public int MusicTrackId { get; set; }
    public string Path { get; set; } // Path to the MP3 file
}