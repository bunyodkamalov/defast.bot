using Defast.Bot.Domain.Common.Entities;

namespace Defast.Bot.Domain.Entities.Common;

public class Tracking : Entity
{
    public string? DocNum { get; set; } 

    public int? DocEntry { get; set; } 
    
    public string? CardCode { get; set; }

    public string? CardName { get; set; }

    public string? U_numberOfCntr { get; set; } 

    public string? U_China_platform { get; set; }

    public string? U_numberPlatformKzx { get; set; }

    public string? U_StationOfOperationRailway { get; set; }

    public string? U_DateOfOperation { get; set; }

    public string? U_LineOfOperation { get; set; }

    public string? U_DestinationStation { get; set; }

    public int? U_Remaining_km { get; set; }

    public string? U_DispatchPlan { get; set; }

    public string? U_DateSending { get; set; }
    
}