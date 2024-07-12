using Defast.Bot.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Defast.Bot.Persistence.DataContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
    public DbSet<IncomingPayment> IncomingPayments => Set<IncomingPayment>();

    public DbSet<IncomingPaymentAccount> IncomingPaymentAccounts => Set<IncomingPaymentAccount>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}