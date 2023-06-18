namespace PersonaScreenshotParser.Core.Models;

public sealed class ScreenshotParsingInput : IDisposable
{
    public string FilePath { get; }
    public Stream ContentStream { get; }
    public int Width { get; }
    public int Height { get; }
 
    public ScreenshotParsingInput(string filePath, Stream contentStream, int width, int height)
    {
        FilePath = filePath;
        ContentStream = contentStream;
        Width = width;
        Height = height;
    }

    public void Dispose()
    {
        ContentStream.Dispose();
    }
}