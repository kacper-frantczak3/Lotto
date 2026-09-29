using LottoGenerator.Api.Data;
using LottoGenerator.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LottoGenerator.Tests;

public class LottoServiceTests
{
private LottoDbContext GetInMemoryDbContext()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<LottoDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new LottoDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Theory]
    [InlineData("lotto", 49, 6)]
    [InlineData("mini", 42, 5)]
    [InlineData("eurojackpot", 50, 5)]
    public async Task GenerateNumbers_ShouldReturnCorrectCountAndRange(string gameType, int maxNumber, int expectedCount)
    {
        using var context = GetInMemoryDbContext();
        var service = new LottoService(context);

        var result = await service.GenerateAndSaveNumbersAsync(gameType, 1);

        Assert.NotNull(result);
        Assert.Single(result.GeneratedLines); 

        var line = result.GeneratedLines[0];
        
        Assert.Equal(expectedCount, line.Count);

        Assert.Equal(expectedCount, line.Distinct().Count()); 
        Assert.All(line, num => 
        {
            Assert.True(num >= 1 && num <= maxNumber);
        });

        var sortedLine = line.OrderBy(n => n).ToList();
        Assert.Equal(sortedLine, line);
    }

    [Fact]
    public async Task GenerateNumbers_RespectsLinesCount()
    {
        using var context = GetInMemoryDbContext();
        var service = new LottoService(context);
        int requestedLines = 4;

        var result = await service.GenerateAndSaveNumbersAsync("lotto", requestedLines);

        Assert.Equal(requestedLines, result.GeneratedLines.Count);
    }
}