using System.Globalization;
using System.Text;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public class HandleOutgoingPaymentConfirmed(
    IIncomingPaymentsService incomingPaymentsService,
    IBusinessPartnerService businessPartnerService,
    IOptions<Chats> cashier)
{
    public async ValueTask Handle(Dictionary<ECurrency, decimal> paymentAmounts, string comment, ELanguage eLanguage,
        ITelegramBotClient tgClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var uzbText = new StringBuilder("Kirim to'lov yaratildi: \n\n" +
                                        "Valyuta: ");
        var rusText = new StringBuilder("Входящий платеж создан: \n\n" +
                                        "Валюта: ");

        var businessPartner = await businessPartnerService.GetByTgIdAsync(callbackQuery.Message!.Chat.Id, cancellationToken);

        foreach (var amount in paymentAmounts)
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append($" {amount.Key.ToString()}\n")
                : rusText.Append($" {amount.Key.ToString()}\n");

            var incomingPayment = new IncomingPayment()
            {
                Id = Guid.NewGuid(),
                CashAccount = amount.Key == ECurrency.USD ? "5030" : "5031",
                CardCode = businessPartner!.CardCode,
                Remarks = comment,
                CardName = businessPartner.CardName,
                DocCurrency = amount.Key.ToString(),
                CashSum = amount.Value
            };

            await incomingPaymentsService.InsertAsync(incomingPayment, cancellationToken);
            
            var confirmingMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            eLanguage == ELanguage.Uzbek ? "Tasdiqlash ✅" : "Подтверждать ✅",
                            $"cashierConfirmed_{incomingPayment.Id}")
                    }
                });

            await tgClient.SendTextMessageAsync(cashier.Value.CashierChatId,
                eLanguage == ELanguage.Uzbek
                    ? $"{(amount.Key == ECurrency.UZS ? "UZS" : "USD")}da kirim to'lov yaratildi: \n\n" +
                      $"Summa: {amount.Value.ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(amount.Key == ECurrency.UZS ? "so'm" : "$")}\n" +
                      $"\nIzoh \ud83d\udcac: \n\n {comment}\n\n" +
                      $"Telefon: {businessPartner.Phone1}\n" +
                      $"CardName: {businessPartner.CardName}"
                      
                    : $"Входящий платеж создан на {(amount.Key == ECurrency.UZS ? "UZS" : "USD")}\n" +
                      $"Сумма: {amount.Value.ToString("#,##.##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(amount.Key == ECurrency.UZS ? "so'm" : "$")}\n" +
                      $"Комментарий \ud83d\udcac: \n\n{comment}\n\n" +
                      $"Телефон: {businessPartner.Phone1}\n" +
                      $"CardName: {businessPartner.CardName}",
                replyMarkup: confirmingMarkup,
                cancellationToken: cancellationToken);
        }

        if (paymentAmounts.ContainsKey(ECurrency.UZS) && paymentAmounts.ContainsKey(ECurrency.USD))
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append(
                    $"UZS da to'lov summasi: {paymentAmounts[ECurrency.UZS].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n" +
                    $"USD da to'lov summasi: {paymentAmounts[ECurrency.USD].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}")
                : rusText.Append(
                    $"\n\nСумма оплаты UZS: {paymentAmounts[ECurrency.UZS].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n" +
                    $"Сумма оплаты USD: {paymentAmounts[ECurrency.USD].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}");
        }
        else if (paymentAmounts.ContainsKey(ECurrency.UZS))
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append(
                    $"\n\nUZS da to'lov summasi: {paymentAmounts[ECurrency.UZS].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n")
                : rusText.Append(
                    $"\n\nСумма оплаты UZS: {paymentAmounts[ECurrency.UZS].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n");
        }
        else if (paymentAmounts.ContainsKey(ECurrency.USD))
        {
            _ = eLanguage == ELanguage.Uzbek
                ? uzbText.Append(
                    $"\n\nUSD da to'lov summasi: {paymentAmounts[ECurrency.USD].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n")
                : rusText.Append(
                    $"\n\nСумма оплаты USD: {paymentAmounts[ECurrency.USD].ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n\n");
        }

        await tgClient.SendTextMessageAsync(callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek
                ? "Chiqim to'lovini kassir qabul qilgach sizga xabar keladi !❗\n" +
                  "Quyidagilardan birini tanlang"
                : "Квитанция отправлена в кассу для подтверждения оплаты.❗\n" +
                  "Выберите один из следующих",
            replyMarkup: UserMainMenuMarkup.Get(eLanguage),
            cancellationToken: cancellationToken);

        await tgClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, cancellationToken);
    }
}