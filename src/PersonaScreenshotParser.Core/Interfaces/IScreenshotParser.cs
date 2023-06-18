using PersonaScreenshotParser.Core.Models;

namespace PersonaScreenshotParser.Core.Interfaces;

public interface IScreenshotParser
{
    public Task<ScreenshotParsingResult> ParseAsync(ScreenshotParsingInput input, CancellationToken ct = default);
}