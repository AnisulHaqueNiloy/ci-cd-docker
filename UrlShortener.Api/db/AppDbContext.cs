using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Entity;
namespace UrlShortener.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ShortUrl> ShortUrls { get; set; }
}