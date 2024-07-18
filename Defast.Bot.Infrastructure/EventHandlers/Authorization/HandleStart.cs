using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.EventHandlers.CashierSide;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.Authorization;

public class HandleStart(IBusinessPartnerService businessPartnerService)
{
    public async ValueTask HandleAsync(ITelegramBotClient tgClient, Message message,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        var businessPartner = await businessPartnerService.GetByTgIdAsync(message.Chat.Id, cancellationToken);
        
        if (businessPartner!.CardCode.Contains("cashier", StringComparison.OrdinalIgnoreCase))
            await tgClient.SendTextMessageAsync(message.Chat.Id,
                eLanguage == ELanguage.Uzbek ? "Foydalanuvchi tasdiqlandi ✅" : "Пользователь подтвержден ✅",
                replyMarkup: CashierMainMenuMarkup.Get(eLanguage),
                cancellationToken: cancellationToken);
        else
            await tgClient.SendTextMessageAsync(message.Chat.Id,
                eLanguage == ELanguage.Uzbek ? "Foydalanuvchi tasdiqlandi ✅" : "Пользователь подтвержден ✅",
                replyMarkup: UserMainMenuMarkup.Get(eLanguage),
                cancellationToken: cancellationToken);
    }
}