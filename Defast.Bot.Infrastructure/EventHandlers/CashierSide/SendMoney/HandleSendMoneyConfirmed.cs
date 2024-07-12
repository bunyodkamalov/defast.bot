using System.Text;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Domain.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public class HandleSendMoneyConfirmed(IOptions<Chats> cashier, IIncomingPaymentsService incomingPaymentsService)
{
    public async ValueTask Handle(Dictionary<ECurrency, decimal> paymentAmounts, string comment, ELanguage eLanguage,
        ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        foreach (var amount in paymentAmounts)
        {
            var incomingPayment = new IncomingPayment()
            {
                Id = Guid.NewGuid(),
                DocType = "rAccount",
                CardCode = amount.Key == ECurrency.USD ? "5030" : "5031",
                CashAccount = amount.Key == ECurrency.USD ? "5010" : "5020",
                CashSum = amount.Value,
                Remarks = comment,
                DocCurrency = amount.Key.ToString(),
                PaymentAccounts = new List<IncomingPaymentAccount>
                {
                    new IncomingPaymentAccount
                    {
                        AccountCode = amount.Key == ECurrency.USD ? "5030" : "5031",
                        SumPaid = amount.Value
                    }
                }
            };

            var inlineKeyBoardMarkup = new InlineKeyboardMarkup(new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Ha \u2705" : "Да \u2705",
                        $"acceptIncomingPayment_{incomingPayment.Id}")
                ],
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Yo'q ❌" : "Нет ❌",
                        $"ignoreIncomingPayment_{incomingPayment.Id}")
                }
            });

            var createdIncomingPayment = await incomingPaymentsService.InsertAsync(incomingPayment, cancellationToken);

            if (createdIncomingPayment is not null)
            {
                await telegramBotClient.SendTextMessageAsync(cashier.Value.GroupChatId,
                    (eLanguage == ELanguage.Uzbek
                        ? new StringBuilder("Pul o'tkazildi:\n\n\"" +
                                            "Tasdiqlaysizmi?\n\n" +
                                            $"Valyuta: {incomingPayment.DocCurrency}\n" +
                                            $"Summa: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n\n" +
                                            $"Izoh \ud83d\udcac: \n{comment}\n\n")
                        : new StringBuilder("Деньги переведены: \n\n" +
                                            "Подтвердите?\n\n" +
                                            $"Валюта: {incomingPayment.DocCurrency}\n" +
                                            $"Сумма: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n\n" +
                                            $"Комментарий \ud83d\udcac: \n{comment}")).ToString(),
                    replyMarkup: inlineKeyBoardMarkup,
                    cancellationToken: cancellationToken);
                
                
                await telegramBotClient.SendTextMessageAsync(
                    callbackQuery.Message!.Chat.Id,
                    eLanguage == ELanguage.Uzbek
                        ? "Moliyaviy to'lov tasdiqlash uchun guruhga yuborildi ❗"
                        : "Расход отправлено в группу на утверждение ❗",
                    cancellationToken: cancellationToken);
            }

            incomingPayment = default;
        }

        await telegramBotClient.SendTextMessageAsync(callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek
                ? "Quyidagilardan birini tanlang: "
                : "Выберите одно из следующего:",
            replyMarkup: CashierMainMenuMarkup.Get(eLanguage),
            cancellationToken: cancellationToken);
    }
}