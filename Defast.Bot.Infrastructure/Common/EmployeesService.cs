using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Identity;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class EmployeesService(
    ICacheBroker cacheBroker,
    IEmployeesRepository employeesRepository,
    IOptions<RequestUrls> requestUris,
    LoginSap loginSap) : IEmployeesService
{
    public async ValueTask<Employee?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetEmployeeByPhoneNumber.Replace("{{mobilePhone}}", phoneNumber.Replace("+", ""));

        return await employeesRepository.GetByPhoneNumberFromSapAsync(url, sessionId!, cancellationToken);
    }
}