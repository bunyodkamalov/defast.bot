using Defast.Bot.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Defast.Bot.Persistence.EntityConfigurations;

public class IncomingPaymentAccountConfiguration : IEntityTypeConfiguration<IncomingPaymentAccount>
{
    public void Configure(EntityTypeBuilder<IncomingPaymentAccount> builder)
    {
        builder.HasKey(e => e.IncomingPaymentId);
    }
}