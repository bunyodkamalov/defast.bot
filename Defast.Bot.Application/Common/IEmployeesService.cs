using Defast.Bot.Domain.Entities.Identity;

namespace Defast.Bot.Application.Common;

public interface IEmployeesService
{
    ValueTask<Employee?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken);
}