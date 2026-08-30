namespace UrlShortener.Api.Services;
using UrlShortener.Api.Entity;
public interface IUrlShortenerService
{
    Task<ShortUrl> CreateShortUrlAsync(string originalUrl);
    Task<ShortUrl?> GetByCodeAsync(string code);
}