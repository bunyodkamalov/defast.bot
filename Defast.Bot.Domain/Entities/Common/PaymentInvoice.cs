
using Newtonsoft.Json;

namespace Defast.Bot.Domain.Entities.Common;

public class PaymentInvoice
{
    public int LineNum { get; set; }
    
    public int DocEntry { get; set; }

    public int DocNum { get; set; }
    
    public decimal SumApplied { get; set; } 
    
    [JsonIgnore]
    public Guid IncomingPaymentId { get; set; }
    
}