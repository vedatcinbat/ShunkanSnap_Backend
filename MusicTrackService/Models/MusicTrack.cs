namespace MusicTrackService.Models;

public class MusicTrack
{
    public int MusicTrackId { get; set; }
    
    public required string Title { get; set; }
    
    public required string Artist { get; set; }
    
    public required string Album { get; set; }
    
    public required string Genre { get; set; }
    
    public required string Duration { get; set; }
    
    public required string Path { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}