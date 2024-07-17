namespace Defast.Bot.Infrastructure.DTOs;

public class BusinessPartnerDto
{
    public string Region { get; set; } = default!;

    public string? MobilePhone { get; set; }

    public decimal PurchasedProduct { get; set; }

    public decimal PaidMoney { get; set; }

    public decimal CurrentAccountBalance { get; set; }

    public string SalesPerson { get; set; }

}