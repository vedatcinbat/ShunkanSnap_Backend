using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Assuming EF Core is used
using MusicTrackService.Models;
using MusicTrackService.Data;
using MusicTrackService.Requests; // Assume you have a DbContext or similar data access layer

namespace MusicTrackService.Services.MusicService
{
    public class MusicTrackService : IMusicTrackService
    {
        private readonly MusicTrackDbContext _context; // Assume this is your DbContext or data layer

        public MusicTrackService(MusicTrackDbContext context)
        {
            _context = context;
        }

        public async Task<MusicTrack> CreateMusicTrackAsync(CreateMusicTrackRequest request)
        {
            var musicTrack = new MusicTrack
            {
                Title = request.Title,
                Artist = request.Artist,
                Album = request.Album,
                Genre = request.Genre,
                Duration = request.Duration,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.MusicTracks.AddAsync(musicTrack);
            await _context.SaveChangesAsync();

            return musicTrack;
        }

        public async Task<MusicTrack> GetMusicTrackByIdAsync(int musicTrackId)
        {
            return await _context.MusicTracks.FindAsync(musicTrackId);
        }

        public async Task<MusicTrack> UpdateMusicTrackAsync(UpdateMusicTrackRequest request)
        {
            var musicTrack = await _context.MusicTracks.FindAsync(request.MusicTrackId);
            if (musicTrack == null)
            {
                throw new InvalidOperationException("Music track not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.Path))
            {
                musicTrack.Path = request.Path;
            }

            musicTrack.UpdatedAt = DateTime.UtcNow;
            _context.MusicTracks.Update(musicTrack);
            await _context.SaveChangesAsync();

            return musicTrack;
        }
    }
}
