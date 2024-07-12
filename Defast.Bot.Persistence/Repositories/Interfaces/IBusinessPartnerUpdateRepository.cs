using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IBusinessPartnerUpdateRepository
{
    ValueTask<BusinessPartnerUpdate?> UpdateAsync(BusinessPartnerUpdate businessPartnerUpdate, string url, string sessionId, CancellationToken cancellationToken); 
}