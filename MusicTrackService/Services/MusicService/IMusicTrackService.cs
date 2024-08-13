using MusicTrackService.Models;
using MusicTrackService.Requests;

namespace MusicTrackService.Services.MusicService;

public interface IMusicTrackService
{
    Task<MusicTrack> CreateMusicTrackAsync(CreateMusicTrackRequest request);
    Task<MusicTrack> GetMusicTrackByIdAsync(int musicTrackId);
    Task<MusicTrack> UpdateMusicTrackAsync(UpdateMusicTrackRequest request);
}