using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class SalesInvoice : Entity
{
    public int? DocNum { get; set; }
    
    public int DocEntry { get; set; }
    
    public string DocCurrency { get; set; }
    
    public string? DocDueDate { get; set; }
    
    public string CardCode { get; set; } = default!;

    public string CardName { get; set; } = default!;
    
    public decimal? PaidToDate { get; set; } 
    
    public decimal? PaidToDateFC { get; set; } 
    
    public string? Comments { get; set; }
    
    public decimal? DocTotal { get; set; }
    
    public decimal? DocTotalFc { get; set; }
    
    public int? U_Driver { get; set; }
    
    public string? U_Father_DocNum { get; set; }
    
    public int? DocumentsOwner { get; set; }
    
    public int SalesPersonCode { get; set; }
    
    public List<InvoiceDocLines> DocumentLines { get; set; } = default!;
}