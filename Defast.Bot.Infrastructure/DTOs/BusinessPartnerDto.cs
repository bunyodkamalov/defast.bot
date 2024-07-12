namespace Defast.Bot.Infrastructure.DTOs;

public class BusinessPartnerDto
{
    public string Region { get; set; } = default!;
    
    public string Realizators { get; set; } = default!;
    
    public string? MobilePhone { get; set; }

    public decimal BalanceFirstDayOfTheMonth { get; set; }

    public decimal PurchasedProduct { get; set; } 
    
    public decimal PaidMoney { get; set; } 
    
    public decimal TotalAmountReceived { get; set; }  
    
    public decimal BalanceLastDayOfTheMonth { get; set; } 
    
    public decimal CurrentAccountBalance { get; set; } 
    
    public string SalesPerson { get; set; } 
    
    public decimal? MoneySpeed { get; set; }
}