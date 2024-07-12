using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class Account : Entity
{
    public string AcctCode { get; set; } = default!;

    public string AcctName { get; set; } = default!;
    
    public decimal CurrTotal { get; set; } = default!;
}