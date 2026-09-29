using System.Security.Cryptography;
using System.Text.Json;
using LottoGenerator.Api.Data;
using LottoGenerator.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LottoGenerator.Api.Services;

public interface ILottoService
{
    Task<LottoResult> GenerateAndSaveNumbersAsync(string gameType, int linesCount);
    Task<List<LottoTicketResponse>> GetHistoryAsync();
}

public record LottoTicketResponse(int Id, string GameType, DateTime GeneratedAt, List<List<int>> Lines);

public class LottoService : ILottoService
{
    private readonly LottoDbContext _context;

    public LottoService(LottoDbContext context)
    {
        _context = context;
    }

    public async Task<LottoResult> GenerateAndSaveNumbersAsync(string gameType, int linesCount)
    {
        var (maxNumber, countToDraw) = gameType.ToLower() switch
        {
            "mini" => (42, 5),
            "eurojackpot" => (50, 5),
            _ => (49, 6)
        };

        linesCount = Math.Clamp(linesCount, 1, 10);
        var allLines = new List<List<int>>();

        for (int i = 0; i < linesCount; i++)
        {
            var line = GenerateSingleLine(maxNumber, countToDraw);
            allLines.Add(line);
        }

        var result = new LottoResult(
            GameType: gameType.ToLower(),
            GeneratedAt: DateTime.UtcNow,
            GeneratedLines: allLines
        );

        var ticket = new LottoTicket
        {
            GameType = result.GameType,
            GeneratedAt = result.GeneratedAt,
            NumbersJson = JsonSerializer.Serialize(allLines)
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<List<LottoTicketResponse>> GetHistoryAsync()
    {
        var tickets = await _context.Tickets
            .OrderByDescending(t => t.GeneratedAt)
            .Take(20) 
            .ToListAsync();

        return tickets.Select(t => new LottoTicketResponse(
            Id: t.Id,
            GameType: t.GameType,
            GeneratedAt: t.GeneratedAt,
            Lines: JsonSerializer.Deserialize<List<List<int>>>(t.NumbersJson) ?? new()
        )).ToList();
    }

    private List<int> GenerateSingleLine(int maxNumber, int countToDraw)
    {
        var numbers = new HashSet<int>();

        while (numbers.Count < countToDraw)
        {
            int randomNumber = RandomNumberGenerator.GetInt32(1, maxNumber + 1);
            numbers.Add(randomNumber);
        }

        return numbers.OrderBy(n => n).ToList();
    }
}