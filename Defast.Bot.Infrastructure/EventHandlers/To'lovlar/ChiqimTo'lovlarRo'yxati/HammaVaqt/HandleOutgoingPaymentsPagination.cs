using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.HammaVaqt;

public class HandleOutgoingPaymentsPagination(IIncomingPaymentsService incomingPaymentsService, IBusinessPartnerService businessPartnerService, ICacheBroker cacheBroker)
{
     public async ValueTask<Message> Handle(int pageToken, ITelegramBotClient botClient, Message paginationMessage,
        ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
     
        var businessPartner = await businessPartnerService.GetByTgIdAsync(paginationMessage.Chat.Id, cancellationToken);

        if (!await cacheBroker.TryGetAsync<List<IncomingPayment>?>("outgoingPayments", out var outgoingPayments, cancellationToken))
        {
            outgoingPayments = await incomingPaymentsService.GetByCardCodeAsync(businessPartner.CardCode, cancellationToken);
            await cacheBroker.SetAsync("outgoingPayments", outgoingPayments, cancellationToken: cancellationToken);
        }
        
        if (!outgoingPayments!.Any())
            return await botClient.SendTextMessageAsync(
                paginationMessage.Chat.Id,
                eLanguage == ELanguage.Uzbek
                    ? "Chiqim to'lovlar topilmadi ❌"
                    : "Исходящие платежи не найдены❌",
                cancellationToken: cancellationToken);
        else
        {
            var messageText = eLanguage == ELanguage.Uzbek
                ? "Chiqim to'lovlar \ud83d\udd04: \n\n"
                : "Исходящие платежи \ud83d\udd04: \n\n";
            
            List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
            foreach (var outgoingPayment in outgoingPayments!.Skip((pageToken - 1) * 10).Take(10))
                inlineKeyboardButtons.Add([
                    InlineKeyboardButton.WithCallbackData($"Hujjat raqami №{outgoingPayment.DocNum}", $"outgoingPaymentdocNum_{outgoingPayment.DocNum}")
                ]);

            InlineKeyboardButton[] inlineKeyBoardArray = new InlineKeyboardButton[outgoingPayments!.Count / 10 + 1];

            for (int i = 1; i <= outgoingPayments!.Count / 10 + 1; i++)
                inlineKeyBoardArray[i - 1] = InlineKeyboardButton.WithCallbackData($"{i}", $"outgoingPaymentPage_{i}");

            inlineKeyboardButtons.Add(inlineKeyBoardArray);
            InlineKeyboardMarkup inlineMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

            await botClient.DeleteMessageAsync(paginationMessage.Chat.Id, paginationMessage.MessageId, cancellationToken);
            
            return await botClient.SendTextMessageAsync(
                paginationMessage.Chat.Id,
                messageText,
                replyMarkup: inlineMarkup,
                cancellationToken: cancellationToken);
        }
    }
}