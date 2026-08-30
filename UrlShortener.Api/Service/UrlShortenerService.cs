namespace UrlShortener.Api.Services;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Entity;
using StackExchange.Redis;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly AppDbContext _context;
    private readonly IConnectionMultiplexer _redis;
    public UrlShortenerService(AppDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _redis =redis;
    }

    public async Task<ShortUrl> CreateShortUrlAsync(string originalUrl)
    {
        var code = GenerateUniqueCode();
        var shortUrl = new ShortUrl
        {
            Code = code,
            OriginalUrl = originalUrl,
            CreatedAt = DateTime.UtcNow
        };
        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync();
        return shortUrl;
    }

       public async Task<ShortUrl?> GetByCodeAsync(string code)
    {
        var db = _redis.GetDatabase();
        var cacheKey = $"shorturl:{code}";

       
        var cachedUrl = await db.StringGetAsync(cacheKey);

        if (cachedUrl.HasValue)
        {
            Console.WriteLine($"✅ CACHE HIT for code: {code}");
            return new ShortUrl
            {
                Code = code,
                OriginalUrl = cachedUrl!
            };
        }

        Console.WriteLine($"❌ CACHE MISS for code: {code} — querying database...");

   
        var shortUrl = await _context.ShortUrls
            .FirstOrDefaultAsync(u => u.Code == code);

        
        if (shortUrl != null)
        {
            await db.StringSetAsync(cacheKey, shortUrl.OriginalUrl, TimeSpan.FromHours(24));
        }

        return shortUrl;
    }
    private static string GenerateUniqueCode()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 6);
    }
}