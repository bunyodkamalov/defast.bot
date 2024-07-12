using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class BusinessPartnerService(
        LoginSap loginSap,
        IBusinessPartnerRepository businessPartnerRepository,
        ICacheBroker cacheBroker,
        IBusinessPartnerUpdateRepository businessPartnerUpdateRepository,
        IOptions<RequestUrls> requestUrls,
        IOptions<HanaSqlUrls> hanaSqlUrls)
    : IBusinessPartnerService
{
    public async ValueTask<List<BusinessPartner?>> GetAsync(CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUrls.Value.BaseUrl + requestUrls.Value.GetBusinessPartners;
        
        return await businessPartnerRepository.GetBusinessPartnersAsync(url, sessionId!, cancellationToken);
    }

    public async ValueTask<BusinessPartner?> GetByCardCodeAsync(string cardCode, string startDate, string endDate, CancellationToken cancellationToken)
    {
        var url = hanaSqlUrls.Value.BaseUrl + hanaSqlUrls.Value.GetBusinessPartner
            .Replace("{{cardCode}}", cardCode)
            .Replace("{{startDate}}", startDate)
            .Replace("{{endDate}}", endDate);
        var result = await businessPartnerRepository.GetBusinessPartnerHanaAsync(url, cancellationToken);
        
        return result.SingleOrDefault();
    }

    public async ValueTask<BusinessPartner?> GetByPhoneNumberAsync(string phone, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

         var url = requestUrls.Value.BaseUrl + requestUrls.Value.GetBpByPhoneNumber.Replace("{{mobilePhone}}", phone);
        
        return await businessPartnerRepository.GetBusinessPartnerAsync(url, sessionId!, cancellationToken);
    }

    public async ValueTask<BusinessPartnerUpdate?> UpdateAsync(string cardCode, long tgId, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUrls.Value.BaseUrl + requestUrls.Value.UpdateBusinessPartner.Replace("{{cardCode}}", cardCode);
        var updateModel = new BusinessPartnerUpdate()
        {
            U_TG_ID = tgId
        };
        
        return await businessPartnerUpdateRepository.UpdateAsync(updateModel, url, sessionId!, cancellationToken);
    }

    public async ValueTask<BusinessPartner?> GetByTgIdAsync(long chatId, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUrls.Value.BaseUrl + requestUrls.Value.GetBpByTgId.Replace("{{tgId}}", chatId.ToString());
        
        return await businessPartnerRepository.GetBusinessPartnerAsync(url, sessionId!, cancellationToken);
    }
}