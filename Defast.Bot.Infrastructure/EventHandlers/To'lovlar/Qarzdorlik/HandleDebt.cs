using System.Globalization;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.Qarzdorlik;

public class HandleDebt(IBusinessPartnerService businessPartnerService)
{
    public async ValueTask Handle(ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        var businessPartner = await businessPartnerService.GetByTgIdAsync(callbackQuery.Message!.Chat.Id, cancellationToken);

        if (businessPartner is not null)
        {
            var messageText = eLanguage == ELanguage.Uzbek
                ? "ℹ️ Ma'lumot\n\n" +
                  $"Ism: {businessPartner!.CardName}\n" +
                  $"Balans: {decimal.Parse(businessPartner.CurrentAccountBalance!).ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')} $\n "
                : "ℹ️ Данные" +
                  $"Имя: {businessPartner!.CardName}" +
                  $"Баланс: {decimal.Parse(businessPartner.CurrentAccountBalance!).ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')} $\n";

            await telegramBotClient.SendTextMessageAsync(
                callbackQuery.Message!.Chat.Id,
                messageText,
                cancellationToken: cancellationToken
            );
        }
    }
}