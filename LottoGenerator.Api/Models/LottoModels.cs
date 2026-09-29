namespace LottoGenerator.Api.Models;

public record LottoRequest(
    string GameType = "lotto", 
    int LinesCount = 1 
);

public record LottoResult(
    string GameType,
    DateTime GeneratedAt,
    List<List<int>> GeneratedLines
);