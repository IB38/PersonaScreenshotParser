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
            var path = args.Length > 0 ? args[0] : PromptInputPath();
            var directoryMode = IsDirectory(path) ??
                                throw new ArgumentException("Input file/directory doesn't exist", nameof(args));

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

            var results = await inputs.Select(async inp =>
            {
                var res = parser.ParseAsync(inp);
                return (inp, await res);
            }).WhenAll();

            var recordCount = await SaveToDb(results);
            
            Console.WriteLine($"Saved '{recordCount}' result to database");
        }

        private static async Task<int> SaveToDb((ScreenshotParsingInput Input, ScreenshotParsingResult Result)[] results)
        {
            await using var ctx = new ParserDbContext("test.db");
            await ctx.Database.EnsureCreatedAsync();

            await ctx.ParsingResults.AddRangeAsync(
                results.Select(pair => 
                    new StoredParsingResult(pair.Input.FilePath, pair.Result.CharacterName, pair.Result.DialogueText)));

            return await ctx.SaveChangesAsync();
        }
        
        private static string PromptInputPath()
        {
            Console.WriteLine("Enter input file/directory path:");
            return Console.ReadLine() ?? "";
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