using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;

    public UrlController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> ShortenUrl([FromBody] ShortenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL is required.");
        }

        var shortUrl = await _urlShortenerService.CreateShortUrlAsync(request.Url);

        return Ok(new
        {
            code = shortUrl.Code,
            originalUrl = shortUrl.OriginalUrl,
            shortUrl = $"{Request.Scheme}://{Request.Host}/{shortUrl.Code}"
        });
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code)
    {
        var shortUrl = await _urlShortenerService.GetByCodeAsync(code);

        if (shortUrl == null)
        {
            return NotFound("Short URL not found.");
        }

        return Redirect(shortUrl.OriginalUrl);
    }
}

public class ShortenRequest
{
    public string Url { get; set; } = string.Empty;
}