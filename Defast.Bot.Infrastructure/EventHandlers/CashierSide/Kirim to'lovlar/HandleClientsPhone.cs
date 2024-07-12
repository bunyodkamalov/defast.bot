using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public class HandleClientsPhone(IBusinessPartnerService businessPartnerService)
{
    public async ValueTask<BusinessPartner> Handle(string phoneNumber, ITelegramBotClient botClient, Message message, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        var businessPartner = await businessPartnerService.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

        if (businessPartner is null)
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                eLanguage == ELanguage.Uzbek ? "Mijoz topilmadi! ❌" : "Клиент не найден! ❌",
                replyMarkup: CashierMainMenuMarkup.Get(eLanguage),
                cancellationToken: cancellationToken);

        var inlineKeyboardMarkup = new InlineKeyboardMarkup(
            new []
            {
                [InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Ha ✅" : "Да ✅", "clientConfirmed")],
                new []{InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Yo'q ❌" : "Нет ❌", "clientNotConfirmed")}
            });
        
        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            eLanguage == ELanguage.Uzbek ? $"Shu mijozni izlagan edingizmi?  \n- {businessPartner!.CardName}" : $"Вы его искали ?\n - {businessPartner!.CardName}",
            replyMarkup: inlineKeyboardMarkup,
            cancellationToken: cancellationToken);

        return businessPartner;
    }
}