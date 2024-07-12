using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Application.Common;

public interface IBusinessPartnerService
{
    ValueTask<List<BusinessPartner?>> GetAsync(CancellationToken cancellationToken);

    ValueTask<BusinessPartner?> GetByCardCodeAsync(string cardCode, string startDate, string endDate, CancellationToken cancellationToken);

    ValueTask<BusinessPartner?> GetByPhoneNumberAsync(string phone, CancellationToken cancellationToken);

    ValueTask<BusinessPartnerUpdate?> UpdateAsync(string cardCode, long tgId, CancellationToken cancellationToken);

    ValueTask<BusinessPartner?> GetByTgIdAsync(long chatId, CancellationToken cancellationToken);
}