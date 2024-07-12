namespace Defast.Bot.Domain.Settings;

public class RequestUrls
{
    public string BaseUrl { get; set; } = default!;

    public string GetEmployeeByPhoneNumber { get; set; } = default!;

    public string GetBusinessPartners { get; set; } = default!;

    public string GetBpByPhoneNumber { get; set; } = default!;

    public string GetBpByTgId { get; set; } = default!;
    
    public string UpdateBusinessPartner { get; set; } = default!;
    
    public string GetWarehouses { get; set; } = default!;

    public string GetWarehouseInfo { get; set; } = default!;

    public string GetCredits { get; set; } = default!;

    public string GetAccounts { get; set; } = default!;

    public string GetIncomingPayments { get; set; } = default!;

    public string GetInvoiceByDocNum { get; set; } = default!;

    public string GetInvoicesByCardCode { get; set; } = default!;

    public string GetTrackings { get; set; } = default!;

    public string GetTrackingsByDocNum { get; set; } = default!;

    public string GetTrackingsByChatId { get; set; } = default!;
}