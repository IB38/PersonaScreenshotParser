using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonaScreenshotParser.Database;

[Index(nameof(StoredParsingResult.InputFilePath))]
public class StoredParsingResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public string InputFilePath { get; set; }
    
    public string CharacterName { get; set; }
    public string DialogueText { get; set; }

    public StoredParsingResult(string inputFilePath, string characterName, string dialogueText)
    {
        InputFilePath = inputFilePath;
        CharacterName = characterName;
        DialogueText = dialogueText;
    }
}