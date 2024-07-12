using Defast.Bot.Domain.Common.Caching;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class BusinessPartnerUpdateRepository(ApplicationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<BusinessPartnerUpdate, ApplicationDbContext>(dbContext, cacheBroker), IBusinessPartnerUpdateRepository
{
    public ValueTask<BusinessPartnerUpdate?> UpdateAsync(BusinessPartnerUpdate businessPartnerUpdate, string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.UpdateFromSap(businessPartnerUpdate, url, sessionId, cancellationToken);
    }
}