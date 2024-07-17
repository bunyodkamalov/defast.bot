using System.Globalization;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Domain.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public class HandleAcceptIncomingPayment(IIncomingPaymentsService incomingPaymentsService, IOptions<Chats> chats)
{
    public async ValueTask Handle(Guid inVendorPaymentId, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery, ELanguage language, CancellationToken cancellationToken)
    {
      
        var incomingPayment = await incomingPaymentsService.GetByIdAsync(inVendorPaymentId, cancellationToken);

        if (incomingPayment is null)
        {
            await telegramBotClient.SendTextMessageAsync(chats.Value.CashierChatId,
                language == ELanguage.Uzbek ? "Bazada chiqim to'lov topilmadi❌" : "Счета-фактуры не найдены❌",
                cancellationToken: cancellationToken);

            await telegramBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                cancellationToken);
            return;
        }
        
        var postedIncomingPayment = await incomingPaymentsService.CreateAsync(incomingPayment, cancellationToken);
        
        if (postedIncomingPayment is not null)
        {
            await incomingPaymentsService.DeleteByIdAsync(inVendorPaymentId, cancellationToken);
            
            await telegramBotClient.SendTextMessageAsync(
                chats.Value.GroupChatId,
                language == ELanguage.Uzbek
                    ? "Pul ko'chirildi✅\n\n" +
                      $"Valyuta: {incomingPayment.DocCurrency}\n" +
                      $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n" +
                      $"Tasdiqladi: {callbackQuery.Message!.From!.FirstName}"
                    : "Платеж создан\n\n" +
                      $"Валюта: {incomingPayment.DocCurrency}\n" +
                      $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n" +
                      $"Подтвердил(а): {callbackQuery.Message!.From!.FirstName}",
                cancellationToken: cancellationToken);

            await telegramBotClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                cancellationToken);
            
            await telegramBotClient.SendTextMessageAsync(
                chats.Value.CashierChatId,
                language == ELanguage.Uzbek
                    ? "Pul o'tkazish tasdiqlandi✅\n\n" +
                      $"Valyuta: {incomingPayment.DocCurrency}\n" +
                      $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n"
                    : "Платеж создан\n\n" +
                      $"Валюта: {incomingPayment.DocCurrency}\n" +
                      $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n",
                cancellationToken: cancellationToken);
        }
        else
        {
            await telegramBotClient.SendTextMessageAsync(
                callbackQuery.Message!.Chat.Id,
                language == ELanguage.Uzbek ? "Chiqim to'lov yaratilmadi❌" : "Расход не создан❌",
                cancellationToken: cancellationToken);
        }
    }
}