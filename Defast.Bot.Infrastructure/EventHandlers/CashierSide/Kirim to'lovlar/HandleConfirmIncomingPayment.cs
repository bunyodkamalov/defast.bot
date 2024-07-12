using System.Globalization;
using System.Text;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public static class HandleConfirmIncomingPayment
{
    public static async ValueTask HandleAsync(BusinessPartner businessPartner, Dictionary<ECurrency, decimal> currencyAmounts,
        ITelegramBotClient tgClient, Message message,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        StringBuilder uzbText = new StringBuilder($"Kiritgan ma'lumotlaringiz to'g'rimi?\n\nMijoz: {businessPartner.CardName} \n\nValyuta:");
        StringBuilder rusText = new StringBuilder($"Вы правильно ввели данные ?\n\nКлиент: {businessPartner.CardName}\n\n Валюта: ");


        var inlineKeyBoardMarkup = new InlineKeyboardMarkup(new[]
        {
            [
                InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Ha \u2705" : "Да \u2705",
                    "incomingPaymentConfirmed")
            ],
            [
                InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Yo'q ❌" : "Нет ❌",
                    "incomingPaymentNotConfirmed")
            ],  
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    eLanguage == ELanguage.Uzbek ? "O'zgartitish \u270f\ufe0f" : "Изменить \u270f\ufe0f",
                    "editIncomingPayment")
            }
        });

        foreach (var currency in currencyAmounts.Keys)
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append($" {currency.ToString()}")
                : rusText.Append($" {currency.ToString()}");
        }

        if (currencyAmounts.ContainsKey(ECurrency.UZS) && currencyAmounts.ContainsKey(ECurrency.USD))
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append($"\n\nUZS da to'lov summasi: {currencyAmounts[ECurrency.UZS].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n" +
                                 $"USD da to'lov summasi: {currencyAmounts[ECurrency.USD].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}")
                : rusText.Append($"\n\nСумма оплаты UZS: {currencyAmounts[ECurrency.UZS].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n" +
                                 $"Сумма оплаты USD: {currencyAmounts[ECurrency.USD].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}");
        }
        else if (currencyAmounts.ContainsKey(ECurrency.UZS))
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append($"\n\nUZS da to'lov summasi: {currencyAmounts[ECurrency.UZS].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n")
                : rusText.Append($"\n\nСумма оплаты UZS: {currencyAmounts[ECurrency.UZS].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n");
        }
        else if (currencyAmounts.ContainsKey(ECurrency.USD))
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append($"\n\nUSD da to'lov summasi: {currencyAmounts[ECurrency.USD].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n")
                : rusText.Append($"\n\nСумма оплаты USD: {currencyAmounts[ECurrency.USD].ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n");
        }


        await tgClient.SendTextMessageAsync(message.Chat.Id,
            eLanguage == ELanguage.Uzbek
                ? uzbText.ToString()
                : rusText.ToString(),
            replyMarkup: inlineKeyBoardMarkup,
            cancellationToken: cancellationToken);
    }
}