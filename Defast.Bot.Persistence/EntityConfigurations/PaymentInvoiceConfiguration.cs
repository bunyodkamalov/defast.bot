using Defast.Bot.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Defast.Bot.Persistence.EntityConfigurations;

public class PaymentInvoiceConfiguration : IEntityTypeConfiguration<PaymentInvoice>
{
    public void Configure(EntityTypeBuilder<PaymentInvoice> builder)
    {
        builder.HasKey(e => e.IncomingPaymentId);
    }
}