using Defast.Bot.Domain.Entities.Identity;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class EmployeesRepository(ApplicationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<Employee, ApplicationDbContext>(dbContext, cacheBroker), IEmployeesRepository
{
    public ValueTask<Employee?> GetByPhoneNumberFromSapAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntityFromUrl(url, sessionId, cancellationToken);
    }
}