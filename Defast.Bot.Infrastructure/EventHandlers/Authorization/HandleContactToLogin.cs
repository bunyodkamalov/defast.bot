using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Entities.Identity;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.EventHandlers.CashierSide;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.Authorization;

public class HandleContactToLogin(IBusinessPartnerService businessPartnerService, ICacheBroker cacheBroker)
{
    public async ValueTask<BusinessPartner?> HandleAsync(ITelegramBotClient tgClient, Message message,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        var businessPartner = await businessPartnerService.GetByPhoneNumberAsync(message.Contact!.PhoneNumber, cancellationToken);

        if (businessPartner is not null)
        {
            if (businessPartner.U_TG_ID is null)
                await businessPartnerService.UpdateAsync(businessPartner.CardCode, message.Chat.Id, cancellationToken);

            if (businessPartner.CardCode.Contains("cashier", StringComparison.OrdinalIgnoreCase))
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
        else
        {
            await tgClient.SendTextMessageAsync(message.Chat.Id,
                text: eLanguage == ELanguage.Uzbek ? "Foydalanuvchi topilmadi ❌" : "Пользователь не найден ❌",
                cancellationToken: cancellationToken);
        }

        return businessPartner;
    }
}