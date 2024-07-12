using Defast.Bot.Domain.Entities.Identity;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IEmployeesRepository
{
    ValueTask<Employee?> GetByPhoneNumberFromSapAsync(string url, string sessionId, CancellationToken cancellationToken);
}