using FluentValidation;
using LottoGenerator.Api.Data;
using LottoGenerator.Api.Models;
using LottoGenerator.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LottoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILottoService, LottoService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LottoDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.MapGet("/", async context =>
{
    var filePath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html");
    if (File.Exists(filePath))
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Plik index.html nie został odnaleziony.");
    }
});

app.MapPost("/api/lotto/generate", async (LottoRequest request, IValidator<LottoRequest> validator, ILottoService lottoService) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    try
    {
        var result = await lottoService.GenerateAndSaveNumbersAsync(request.GameType, request.LinesCount);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GenerateLottoNumbers");

app.MapGet("/api/lotto/history", async (ILottoService lottoService) =>
{
    var history = await lottoService.GetHistoryAsync();
    return Results.Ok(history);
})
.WithName("GetLottoHistory");

app.MapDelete("/api/lotto/history", async (LottoDbContext context) =>
{
    context.Tickets.RemoveRange(context.Tickets);
    await context.SaveChangesAsync();
    return Results.Ok(new { message = "Historia została wyczyszczona." });
})
.WithName("ClearLottoHistory");

app.Run();