using Microsoft.EntityFrameworkCore;

namespace PersonaScreenshotParser.Database;

public class ParserDbContext : DbContext
{
    private readonly string _sqlLiteDbPath;
    
    public ParserDbContext(string sqlLiteDbPath)
    {
        _sqlLiteDbPath = sqlLiteDbPath;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
 
        optionsBuilder.UseSqlite($@"DataSource={_sqlLiteDbPath};");
    }

    public DbSet<StoredParsingResult> ParsingResults { get; set; }
}