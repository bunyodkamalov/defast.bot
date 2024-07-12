using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Identity;

public class Employee : Entity
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string JobTitle { get; set; } = default!;

    public string MobilePhone { get; set; } = default!;
}