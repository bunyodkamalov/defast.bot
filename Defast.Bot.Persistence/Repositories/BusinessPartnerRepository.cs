using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class BusinessPartnerRepository(ApplicationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<BusinessPartner, ApplicationDbContext>(dbContext, cacheBroker), IBusinessPartnerRepository
{
    public ValueTask<List<BusinessPartner?>> GetBusinessPartnersAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntitiesFromUrl(url, sessionId, cancellationToken);
    }

    public ValueTask<List<BusinessPartner?>> GetBusinessPartnerHanaAsync(string url, CancellationToken cancellationToken)
    {
        return base.GetQueryResultWithNoPagination(url, cancellationToken);
    }

    public ValueTask<BusinessPartner?> GetBusinessPartnerAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntityFromUrl(url, sessionId, cancellationToken);
    }

    public ValueTask<BusinessPartner?> UpdateBusinessPartnerAsync(BusinessPartner businessPartner, string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.UpdateFromSap(businessPartner, url, sessionId, cancellationToken);
    }
}