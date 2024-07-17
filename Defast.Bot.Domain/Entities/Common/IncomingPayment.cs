using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class IncomingPayment : Entity
{
    public string? DocType { get; set; }
    
    public string? DocDate { get; set; }
    
    public int? DocNum { get; set; }
    
    public string? CashAccount { get; set; }
    
    public string DocCurrency { get; set; } = default!;

    public decimal CashSum { get; set; } = default!;
    
    public decimal? CashSumFC { get; set; }

    public string? Remarks { get; set; } = default!;

    public string CardCode { get; set; } = default!;
    public string? CardName { get; set; }

    public string U_cashFlow { get; set; } = "4";

    public List<IncomingPaymentAccount>? PaymentAccounts { get; set; } = new();
    
    public List<PaymentInvoice>? PaymentInvoices { get; set; } = new ();
}