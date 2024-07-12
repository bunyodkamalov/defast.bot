using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class Warehouse : Entity
{
    public string? WarehouseCode { get; set; } = default!;

    public string? WarehouseName { get; set; } = default!;

    public string? InvntryUom { get; set; } = default!;

    public string? ItemName { get; set; } = default!;
    
    public decimal? OnHand { get; set; } 
}