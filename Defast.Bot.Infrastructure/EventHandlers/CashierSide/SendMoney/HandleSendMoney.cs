using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public static class HandleSendMoney
{
    public static async ValueTask Handle(ITelegramBotClient botClient, CallbackQuery callbackQuery, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        var inlineMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData("UZS \ud83c\uddfa\ud83c\uddff", "UZSsendMoney"),
                    InlineKeyboardButton.WithCallbackData("USD \ud83c\uddfa\ud83c\uddf8", "USDsendMoney")
                ],
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        eLanguage == ELanguage.Uzbek
                            ? "UZS va USD \ud83c\uddfa\ud83c\uddff\ud83c\uddfa\ud83c\uddf8"
                            : "UZS и USD \ud83c\uddfa\ud83c\uddff\ud83c\uddfa\ud83c\uddf8",
                        "UZS&USDsendMoney")
                }
            });

        var messageText = eLanguage == ELanguage.Uzbek
            ? "To'lov valyutasini tanlang \ud83d\udcb1"
            : "Выберите валюту платежа \ud83d\udcb1";

        await botClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            messageText,
            replyMarkup: inlineMarkup,
            cancellationToken: cancellationToken
        );
    }
}