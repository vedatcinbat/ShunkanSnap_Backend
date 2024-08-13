using Microsoft.AspNetCore.Mvc;
using MusicTrackService.Responses;
using MusicTrackService.Services.MusicService;

namespace MusicTrackService.Controllers;

[ApiController]
[Route("/api/music-tracks")]
public class MusicTrackController : ControllerBase
{
    private readonly IMusicTrackService _musicTrackService;
    
    public MusicTrackController(IMusicTrackService musicTrackService)
    {
        _musicTrackService = musicTrackService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMusicTracksAsync()
    {
        var musicTracks = await _musicTrackService.QueryMusicTracksAsync();
        
        var queryMusicTrackResponse = musicTracks.Select(musicTrack => new QueryMusicTrackResponse
        {
            MusicTrackId = musicTrack.MusicTrackId,
            Title = musicTrack.Title,
            Artist = musicTrack.Artist,
            Album = musicTrack.Album,
            Genre = musicTrack.Genre,
            Duration = musicTrack.Duration,
            Path = musicTrack.Path,
            CreatedAt = musicTrack.CreatedAt,
            UpdatedAt = musicTrack.UpdatedAt
        });
        
        return Ok(queryMusicTrackResponse);
    }
    
    [HttpGet("{musicTrackId}")]
    public async Task<IActionResult> GetMusicTrackByIdAsync(int musicTrackId)
    {
        var musicTrack = await _musicTrackService.GetMusicTrackByIdAsync(musicTrackId);
        
        if (musicTrack == null)
        {
            return NotFound("Music track not found");
        }
        
        return Ok(musicTrack);
    }
}