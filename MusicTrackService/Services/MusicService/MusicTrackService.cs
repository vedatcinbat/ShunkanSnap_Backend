using Microsoft.EntityFrameworkCore;
using MusicTrackService.Data;
using MusicTrackService.Models;

namespace MusicTrackService.Services.MusicService;

public class MusicTrackService : IMusicTrackService
{
    private readonly MusicTrackDbContext _dbContext;
    
    public MusicTrackService(MusicTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<MusicTrack>> QueryMusicTracksAsync()
    {
        return await _dbContext.MusicTracks.ToListAsync();
    }
    
    public async Task<MusicTrack?> GetMusicTrackByIdAsync(int musicTrackId)
    {
        return await _dbContext.MusicTracks.FindAsync(musicTrackId);
    }
}