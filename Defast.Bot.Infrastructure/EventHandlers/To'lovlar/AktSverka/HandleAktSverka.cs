using System.Globalization;
using AutoMapper;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.DTOs;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.AktSverka;

public class HandleAktSverka(IBusinessPartnerService businessPartnerService, IMapper mapper)
{
    public async ValueTask HandleAsync(ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage language, CancellationToken cancellationToken)
    {
        var startDate = DateOnly.FromDateTime(DateTime.MinValue).ToString("dd.MM.yyyy");
        var endDate = DateOnly.FromDateTime(DateTime.MaxValue).ToString("dd.MM.yyyy");
        
        var bp = await businessPartnerService.GetByTgIdAsync(callbackQuery.Message!.Chat.Id, cancellationToken);

        var businessPartner = await businessPartnerService.GetByCardCodeAsync(bp.CardCode, startDate, endDate, cancellationToken);
        
        if (businessPartner is not null)
        {
            var data = mapper.Map<BusinessPartnerDto>(businessPartner);
            await telegramBotClient.SendTextMessageAsync(callbackQuery.Message!.Chat.Id, 
                $"Ism: {businessPartner.CardName}\n" +
                $"Telefon Raqam: {businessPartner.MobilePhone}\n\n" +
                $"Eltish xizmat narxi:   {data.PurchasedProduct.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $ \n" +
                $"To'langan pul:   {data.PaidMoney.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $ \n" +
                $"To'lanishi kerak:   {data.CurrentAccountBalance.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $ \n",
                replyMarkup: UserMainMenuMarkup.Get(language),
                cancellationToken: cancellationToken);
        }
    }
}