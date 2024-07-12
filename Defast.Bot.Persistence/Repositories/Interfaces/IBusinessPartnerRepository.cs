using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IBusinessPartnerRepository
{
    ValueTask<List<BusinessPartner?>> GetBusinessPartnersAsync(string url, string sessionId, CancellationToken cancellationToken);
    
    ValueTask<List<BusinessPartner?>> GetBusinessPartnerHanaAsync(string url, CancellationToken cancellationToken);
    
    ValueTask<BusinessPartner?> GetBusinessPartnerAsync(string url, string sessionId, CancellationToken cancellationToken);

    ValueTask<BusinessPartner?> UpdateBusinessPartnerAsync(BusinessPartner businessPartner, string url, string sessionId, CancellationToken cancellationToken);
}