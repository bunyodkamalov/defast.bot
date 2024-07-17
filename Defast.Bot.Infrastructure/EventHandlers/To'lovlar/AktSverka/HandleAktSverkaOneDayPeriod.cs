using System.Globalization;
using AutoMapper;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.DTOs;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.AktSverka;

public class HandleAktSverkaOneDayPeriod(IBusinessPartnerService businessPartnerService, IMapper mapper)
{
    public async ValueTask HandleAsync(ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage language, CancellationToken cancellationToken)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today).ToString("dd.MM.yyyy");
        var endDate = DateOnly.FromDateTime(DateTime.Today).ToString("dd.MM.yyyy");
        
        var bp = await businessPartnerService.GetByTgIdAsync(callbackQuery.Message!.Chat.Id, cancellationToken);

        var businessPartner = await businessPartnerService.GetByCardCodeAsync(bp.CardCode, startDate, endDate, cancellationToken);

        if (businessPartner is not null)
        {
            var data = mapper.Map<BusinessPartnerDto>(businessPartner);
            await telegramBotClient.SendTextMessageAsync(callbackQuery.Message!.Chat.Id, 
                $"Davr boshidagi qoldiq:   {data.BalanceFirstDayOfTheMonth.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $\n" +
                $"Sotib olingan mol:   {data.PurchasedProduct.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $ \n" +
                $"To'langan pul:   {data.PaidMoney.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $ \n" +
                $"Jami to'lanishi kerak:   {data.BalanceLastDayOfTheMonth.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')} $ \n" +
                $"Jami qarzdorlik:   {data.CurrentAccountBalance.ToString("00.00", CultureInfo.InvariantCulture).Replace(',', ' ')}$ \n" +
                $"Pul aylanishi: {data.MoneySpeed}\n\n" +
                $"Sana: {startDate}",
                replyMarkup: UserMainMenuMarkup.Get(language),
                cancellationToken: cancellationToken);
        }
    }
}