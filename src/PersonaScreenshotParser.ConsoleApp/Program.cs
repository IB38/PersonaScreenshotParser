using CodeJam;
using CodeJam.Threading;
using PersonaScreenshotParser.Core;
using PersonaScreenshotParser.Core.Models;
using PersonaScreenshotParser.Database;

namespace PersonaScreenshotParser.ConsoleApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var path = args.Length > 0 ? args[0] : Prompt("Enter input file/directory path:");
            var directoryMode = IsDirectory(path) ??
                                throw new ArgumentException("Input file/directory doesn't exist", nameof(args));
            var nukeOldResults = PromptNukeFlag();
            
            using var inputs = new DisposableList<ScreenshotParsingInput>();
            if (directoryMode)
            {
                inputs.AddRange(
                    Directory.GetFiles(path).Select(
                        // TODO: Read real dimensions
                    p => new ScreenshotParsingInput(p, new FileStream(p, FileMode.Open), 0, 0)
                    )
                );
            }
            else
            {
                inputs.Add(new ScreenshotParsingInput(path, new FileStream(path, FileMode.Open), 0, 0));
            }

            var parser = new IronOcrScreenshotParser();

            var take = 100;
            var skip = 0;
            var results = new List<(ScreenshotParsingInput Input, ScreenshotParsingResult Result)>();


            while (true)
            {
                Console.WriteLine($"Skip '{skip}', take '{take}'");
                var rs =
                    await inputs.Skip(skip).Take(take).Select(async inp =>
                {
                    var res = parser.ParseAsync(inp);
                    return (inp, await res);
                }).WhenAll();

                results.AddRange(rs);
                if (rs.Length < take)
                    break;
                
                skip += take;
            }
            
            var now = DateTimeOffset.Now;
            var recordCount = await SaveToDb(results, now, nukeOldResults);
            
            Console.WriteLine($"Saved '{recordCount}' result to database");
        }

        private static async Task<int> SaveToDb(
            IEnumerable<(ScreenshotParsingInput Input, ScreenshotParsingResult Result)> results, 
            DateTimeOffset parsingDt, 
            bool nukeOldResults)
        {
            await using var ctx = new ParserDbContext("test.db");
            if(nukeOldResults) 
                await ctx.Database.EnsureDeletedAsync();
            await ctx.Database.EnsureCreatedAsync();

            await ctx.ParsingResults.AddRangeAsync(
                results.Select(pair => 
                    new StoredParsingResult(pair.Input.FilePath, pair.Result.CharacterName, pair.Result.DialogueText, parsingDt)));

            return await ctx.SaveChangesAsync();
        }
        
        private static string Prompt(string displayedText)
        {
            Console.WriteLine(displayedText);
            return Console.ReadLine() ?? "";
        }

        private static bool PromptNukeFlag()
        {
            const string text = "Nuke old results from DB? y/n";
            var nukeStr = Prompt(text);
            while (IsYes(nukeStr) is null)
            {
                nukeStr = Prompt(text);
            }

            return IsYes(nukeStr)!.Value;

            bool? IsYes(string str)
            {
                return str.ToLower() switch
                {
                    "y" => true,
                    "n" => false,
                    _ => null
                };
            }
        }

        private static bool? IsDirectory(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                return attr.HasFlag(FileAttributes.Directory);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }
    }
}