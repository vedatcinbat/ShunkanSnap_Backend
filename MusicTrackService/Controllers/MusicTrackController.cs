using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicTrackService.Models;
using MusicTrackService.Responses;
using MusicTrackService.Services.MusicService;
using System;
using System.IO;
using System.Threading.Tasks;
using MusicTrackService.Requests;

namespace MusicTrackService.Controllers
{
    [ApiController]
    [Route("/api/music-tracks")]
    public class MusicTrackController : ControllerBase
    {
        private readonly IMusicTrackService _musicTrackService;

        public MusicTrackController(IMusicTrackService musicTrackService)
        {
            _musicTrackService = musicTrackService;
        }

        // POST: /api/music-tracks
        [HttpPost]
        public async Task<IActionResult> CreateMusicTrackAsync([FromBody] CreateMusicTrackRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTrack = await _musicTrackService.CreateMusicTrackAsync(request);

            var response = new QueryMusicTrackResponse
            {
                MusicTrackId = newTrack.MusicTrackId,
                Title = newTrack.Title,
                Artist = newTrack.Artist,
                Album = newTrack.Album,
                Genre = newTrack.Genre,
                Duration = newTrack.Duration,
                Path = newTrack.Path,
                ImagePath = newTrack.ImagePath,
                CreatedAt = newTrack.CreatedAt,
                UpdatedAt = newTrack.UpdatedAt
            };

            return Ok(response);
        }
        
        [HttpGet("{musicTrackId}/download-mp3")]
        public async Task<IActionResult> DownloadMp3Async(int musicTrackId)
        {
            var musicTrack = await _musicTrackService.GetMusicTrackByIdAsync(musicTrackId);
    
            if (musicTrack == null || string.IsNullOrWhiteSpace(musicTrack.Path))
            {
                return NotFound("Music track or MP3 file not found.");
            }
    
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", musicTrack.Path.TrimStart('/'));
    
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("MP3 file not found on server.");
            }
    
            // Return the file for download
            var contentType = "audio/mpeg";
            return PhysicalFile(filePath, contentType, Path.GetFileName(filePath));
        }

        // PATCH: /api/music-tracks/{musicTrackId}/upload-mp3
        [HttpPatch("{musicTrackId}/upload-mp3")]
        public async Task<IActionResult> UploadMp3Async(int musicTrackId, IFormFile mp3File)
        {
            if (mp3File == null || mp3File.Length == 0)
            {
                return BadRequest("No MP3 file provided.");
            }

            var fileExtension = Path.GetExtension(mp3File.FileName).ToLower();
            if (fileExtension != ".mp3")
            {
                return BadRequest("The file must be an MP3.");
            }

            var musicTrack = await _musicTrackService.GetMusicTrackByIdAsync(musicTrackId);
            if (musicTrack == null)
            {
                return NotFound("Music track not found.");
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mp3");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var fileName = $"{musicTrackId}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mp3File.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            musicTrack.Path = $"/mp3/{fileName}";
            try
            {
                await _musicTrackService.UpdateMusicTrackAsync(new UpdateMusicTrackRequest()
                {
                    MusicTrackId = musicTrackId,
                    Path = musicTrack.Path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update music track: {ex.Message}");
            }

            var response = new QueryMusicTrackResponse
            {
                MusicTrackId = musicTrack.MusicTrackId,
                Title = musicTrack.Title,
                Artist = musicTrack.Artist,
                Album = musicTrack.Album,
                Genre = musicTrack.Genre,
                Duration = musicTrack.Duration,
                Path = musicTrack.Path,
                ImagePath = musicTrack.ImagePath,
                CreatedAt = musicTrack.CreatedAt,
                UpdatedAt = musicTrack.UpdatedAt
            };

            return Ok(response);
        }

        // GET: /api/music-tracks/{musicTrackId}
        [HttpGet("{musicTrackId}")]
        public async Task<IActionResult> GetMusicTrackByIdAsync(int musicTrackId)
        {
            var musicTrack = await _musicTrackService.GetMusicTrackByIdAsync(musicTrackId);

            if (musicTrack == null)
            {
                return NotFound("Music track not found");
            }

            var response = new QueryMusicTrackResponse
            {
                MusicTrackId = musicTrack.MusicTrackId,
                Title = musicTrack.Title,
                Artist = musicTrack.Artist,
                Album = musicTrack.Album,
                Genre = musicTrack.Genre,
                Duration = musicTrack.Duration,
                Path = musicTrack.Path,
                ImagePath = musicTrack.ImagePath,
                CreatedAt = musicTrack.CreatedAt,
                UpdatedAt = musicTrack.UpdatedAt
            };

            return Ok(response);
        }
    }
}
