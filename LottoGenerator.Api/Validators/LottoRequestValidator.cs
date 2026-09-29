using FluentValidation;
using LottoGenerator.Api.Models;

namespace LottoGenerator.Api.Validators;

public class LottoRequestValidator : AbstractValidator<LottoRequest>
{
    private static readonly string[] AllowedGames = { "lotto", "mini", "eurojackpot" };

    public LottoRequestValidator()
    {
        RuleFor(x => x.GameType)
            .NotEmpty().WithMessage("Typ gry jest wymagany.")
            .Must(game => AllowedGames.Contains(game.ToLower()))
            .WithMessage($"Nieobsługiwanany typ gry. Dozwolone: {string.Join(", ", AllowedGames)}");

        RuleFor(x => x.LinesCount)
            .InclusiveBetween(1, 10)
            .WithMessage("Liczba kuponów musi mieścić się w przedziale od 1 do 10.");
    }
}