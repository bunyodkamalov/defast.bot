using Newtonsoft.Json;

namespace Defast.Bot.Domain.Entities.Common;

public class IncomingPaymentAccount
{
    public string AccountCode { get; set; } = default!;
    
    public decimal SumPaid { get; set; }
    
    [JsonIgnore]
    public Guid? IncomingPaymentId { get; set; }
}