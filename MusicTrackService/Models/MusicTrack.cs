namespace MusicTrackService.Models;

public class MusicTrack
{
    public int MusicTrackId { get; set; }
    
    public string? Title { get; set; }
    
    public string? Artist { get; set; }
    
    public string? Album { get; set; }
    
    public string? Genre { get; set; }
    
    public string? Duration { get; set; }
    
    public string? Path { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}