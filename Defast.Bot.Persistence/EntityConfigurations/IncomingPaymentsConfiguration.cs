using Defast.Bot.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Defast.Bot.Persistence.EntityConfigurations;

public class IncomingPaymentsConfiguration : IEntityTypeConfiguration<IncomingPayment>
{
    public void Configure(EntityTypeBuilder<IncomingPayment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.PaymentInvoices)
            .WithOne()
            .HasForeignKey(pi => pi.IncomingPaymentId);
        
        builder.HasMany(e => e.PaymentAccounts)
            .WithOne()
            .HasForeignKey(pi => pi.IncomingPaymentId);
    }
}