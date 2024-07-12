using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public static class HandleAddIncomingPayment
{
    public static async ValueTask HandleAsync(ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        await telegramBotClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek
                ? "Kirim to'lov qilmoqchi bo'lgan mijozni telefon raqamini yozing(+998xxxxxxxxx)"
                : "Введите номер клиента(+998xxxxxxxxx)",
            cancellationToken: cancellationToken);
    } 
}