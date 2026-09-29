namespace LottoGenerator.Api.Models;

public class LottoTicket
{
    public int Id { get; set; }
    public string GameType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string NumbersJson { get; set; } = string.Empty; 
}