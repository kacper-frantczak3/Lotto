using Microsoft.EntityFrameworkCore;
using LottoGenerator.Api.Models;

namespace LottoGenerator.Api.Data;

public class LottoDbContext : DbContext
{
    public LottoDbContext(DbContextOptions<LottoDbContext> options) : base(options) { }

    public DbSet<LottoTicket> Tickets => Set<LottoTicket>();
}