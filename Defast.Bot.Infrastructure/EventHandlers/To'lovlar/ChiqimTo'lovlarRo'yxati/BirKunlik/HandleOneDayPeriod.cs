using System.Globalization;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirKunlik;

public class HandleOneDayPeriod(ICacheBroker cacheBroker,
    IBusinessPartnerService businessPartnerService,
    IIncomingPaymentsService incomingPaymentsService)
{
    public async ValueTask<Message> Handle(int pageToken, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery,
        ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        

        var businessPartner = await businessPartnerService.GetByTgIdAsync(callbackQuery.Message!.Chat.Id, cancellationToken);
        var outgoingPayments = await incomingPaymentsService.GetByCardCodeAsync(businessPartner.CardCode, cancellationToken);
       
        if (!outgoingPayments.Any())
            return await telegramBotClient.SendTextMessageAsync(
                callbackQuery.Message!.Chat.Id,
                eLanguage == ELanguage.Uzbek
                    ? "Chiqim to'lovlar topilmadi ❌"
                    : "Исходящие платежи не найдены❌",
                cancellationToken: cancellationToken);
        else
        {
            foreach (var outgoingPayment in outgoingPayments.ToList())
                if (!(DateTime.Parse(outgoingPayment!.DocDate!) > DateTime.Now.AddDays(-1)))
                    outgoingPayments?.Remove(outgoingPayment);
            
            if (!outgoingPayments!.Any())
                return await telegramBotClient.SendTextMessageAsync(
                    callbackQuery.Message!.Chat.Id,
                    eLanguage == ELanguage.Uzbek
                        ? "Bir kunlik chiqim to'lovlar topilmadi ❌"
                        : "Исходящие платежи не найдены❌",
                    cancellationToken: cancellationToken);
           
            await cacheBroker.SetAsync("oneDayPeriodOutgoingPayments", outgoingPayments,
                cancellationToken: cancellationToken);
            
            var messageText = eLanguage == ELanguage.Uzbek
                ? "Bir kunlik chiqim to'lovlar \ud83d\udd04: \n\n"
                : "Платежи за один день \ud83d\udd04: \n\n";
            
            List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
            foreach (var outgoingPayment in outgoingPayments!.Skip((pageToken - 1) * 10).Take(10))
                inlineKeyboardButtons.Add([
                    InlineKeyboardButton.WithCallbackData(
                        $"{DateTimeOffset.Parse(outgoingPayment.DocDate!):dd.MM.yyyy} | {(outgoingPayment.DocCurrency == ECurrency.USD.ToString() 
                            ? $"{outgoingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} $" 
                            : $"{((decimal)outgoingPayment.CashSumFC!).ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} so'm")}",
                        $"outgoingPaymentdocNum_{outgoingPayment.DocNum}")
                ]);

            InlineKeyboardButton[] inlineKeyBoardArray = new InlineKeyboardButton[outgoingPayments!.Count / 10 + 1];

            for (int i = 1; i <= outgoingPayments.Count / 10 + 1; i++)
                inlineKeyBoardArray[i - 1] = InlineKeyboardButton.WithCallbackData($"{i}", $"outgoingPaymentdocNum_{i}");

            inlineKeyboardButtons.Add(inlineKeyBoardArray);
            InlineKeyboardMarkup inlineMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

            return await telegramBotClient.SendTextMessageAsync(
                callbackQuery.Message!.Chat.Id,
                messageText,
                replyMarkup: inlineMarkup,
                cancellationToken: cancellationToken);
        }
    }
}