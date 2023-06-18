namespace PersonaScreenshotParser.Core.Models;

public class ScreenshotParsingResult
{
    public string CharacterName { get; }
    public string DialogueText { get; }

    public ScreenshotParsingResult(string characterName, string dialogueText)
    {
        CharacterName = characterName;
        DialogueText = dialogueText;
    }
}