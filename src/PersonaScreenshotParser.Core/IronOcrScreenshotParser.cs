using IronOcr;
using PersonaScreenshotParser.Core.Interfaces;
using PersonaScreenshotParser.Core.Models;

namespace PersonaScreenshotParser.Core;

public class IronOcrScreenshotParser : IScreenshotParser
{
    public async Task<ScreenshotParsingResult> ParseAsync(ScreenshotParsingInput input, CancellationToken ct = default)
    {
        var ocrEngine = new IronTesseract(new TesseractConfiguration {  BlackListCharacters = "=[]"})
        {
            Language = OcrLanguage.EnglishBest, MultiThreaded = true
        };
        
        using var ocrInput = new OcrInput();
        ocrInput.AddImage(input.ContentStream, 
            // TODO: Calculate dialogue box coordinates for non 1440p resolutions
            new IronSoftware.Drawing.CropRectangle(74, 1024, 1600, 388));

        // Optionally Apply Filters if needed:
        // ocrInput.Deskew();  // use only if image not straight
        // ocrInput.DeNoise(); // use only if image contains digital noise

        var ocrResult = await ocrEngine.ReadAsync(ocrInput);

        string name = "", text = "";
        var validLines = ocrResult.Lines.Where(l => l.Confidence >= 50);
        var foundNameLine = false;
            
        foreach(var l in validLines)
        {
            if(foundNameLine)
            {
                text += l.Text + Environment.NewLine;
                continue;
            }

            if(IsValidNameLine(l))
            {
                foundNameLine = true;
                name = l.Text;
            }
        }

        return new ScreenshotParsingResult(name, text);
    }
    
    private static bool IsValidNameLine(OcrResult.Line l)
    {
        if (l.Words.Length is < 1 or > 2)
            return false;

        return l.Words.All(w => char.IsUpper(w.Text.First()));
    }

    private static void PrintLine(OcrResult.Line l, bool isNameLine = false)
    {
        var content = l.Text.Replace('|', 'I');

        Console.WriteLine(isNameLine ? $"[{content}]" : content);
    }
}