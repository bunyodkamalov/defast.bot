using AutoMapper;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Infrastructure.DTOs;

namespace Defast.Bot.Infrastructure.Mappers;

public class BusinessPartnerDataMapper : Profile
{
    public BusinessPartnerDataMapper()
    {
        CreateMap<BusinessPartner, BusinessPartnerDto>()
            .ForMember(dest => dest.BalanceFirstDayOfTheMonth, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.BalanceFirstDayOfPeriod)))
            .ForMember(dest => dest.PurchasedProduct, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.PurchasedProduct)))
            .ForMember(dest => dest.PaidMoney, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.PaidMoney)))
            .ForMember(dest => dest.TotalAmountReceived, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.TotalAmountReceived)))
            .ForMember(dest => dest.BalanceLastDayOfTheMonth, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.BalanceLastDayOfPeriod)))
            .ForMember(dest => dest.CurrentAccountBalance, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.CurrentAccountBalance)))
            .ForMember(dest => dest.MoneySpeed, 
                opt => opt.MapFrom(src => ConvertToDecimal(src.MoneySpeed)));
    }

    private decimal ConvertToDecimal(string value)
    {
        if (value is not null && decimal.TryParse(value, out var result))
            return Math.Round(result, 2);
        
        return 0m;
    }
}