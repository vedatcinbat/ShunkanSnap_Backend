using Microsoft.EntityFrameworkCore;
using MusicTrackService.Models;

namespace MusicTrackService.Data;

public class MusicTrackDbContext : DbContext
{
    public MusicTrackDbContext(DbContextOptions<MusicTrackDbContext> options) : base(options) {}

    public DbSet<MusicTrack> MusicTracks { get; set; }
}