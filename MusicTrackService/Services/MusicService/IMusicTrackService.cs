using MusicTrackService.Models;

namespace MusicTrackService.Services.MusicService;

public interface IMusicTrackService
{
    Task<List<MusicTrack>> QueryMusicTracksAsync();
    
    Task<MusicTrack?> GetMusicTrackByIdAsync(int musicTrackId);
}