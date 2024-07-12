
using Newtonsoft.Json;

namespace Defast.Bot.Domain.Entities.Common;

public class InvoiceDocLines
{
    public string? ItemDescription { get; set; }
    
    public string ItemCode { get; set; } = default!;

    public int? BaseType { get; set; } = 15;
    
    public int? BaseEntry { get; set; } 
    
    public int? BaseLine { get; set; } 
    
    [JsonProperty("UnitPrice")]
    public decimal Price { get; set; } = default!;

    public string Currency { get; set; } = default!;
    
    public decimal Quantity { get; set; } = default!;
    
    public decimal? InventoryQuantity { get; set; }
    
    public string? WarehouseCode { get; set; }
    
    public decimal? OpenAmount { get; set; } 
    
    public decimal? OpenAmountFC { get; set; } 
    
    public decimal? PaidToDate { get; set; } 
    
    public decimal? PaidToDateFC { get; set; } 
    
    public decimal? RowTotalFC { get; set; }
}