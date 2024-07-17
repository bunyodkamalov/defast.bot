using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class BusinessPartner : Entity
{
    public string CardName { get; set; } = default!;

    public string CardCode { get; set; } = default!;
    
    public string? MobilePhone { get; set; } 
    
    public string? Phone1 { get; set; }
    
    public long? U_TG_ID { get; set; }
    
    public string? PurchasedProduct { get; set; } 
    
    public string? PaidMoney { get; set; } 
    
    public string? CurrentAccountBalance { get; set; } 
    
}