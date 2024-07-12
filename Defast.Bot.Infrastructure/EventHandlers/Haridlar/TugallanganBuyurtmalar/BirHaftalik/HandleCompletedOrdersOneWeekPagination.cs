using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirHaftalik;

public class HandleCompletedOrdersOneWeekPagination(
    IBusinessPartnerService businessPartnerService,
    IInvoicesService invoicesService,
    ICacheBroker cacheBroker)
{
    public async ValueTask<Message> Handle(int pageToken, ITelegramBotClient botClient, Message paginationMessage,
        ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync<List<BusinessPartner?>>("users", out var users, cancellationToken))
        {
            users = await businessPartnerService.GetAsync(cancellationToken);
            await cacheBroker.SetAsync("users", users, cancellationToken: cancellationToken);
        }

        var businessPartner = users.First(bp => bp!.U_TG_ID == paginationMessage.Chat.Id);

        if (!await cacheBroker.TryGetAsync<List<SalesInvoice?>>("completedOrders", out var orders, cancellationToken: cancellationToken))
            orders = await invoicesService.GetInvoicesByCardCodeAsync(businessPartner!.CardCode, cancellationToken);

        foreach (var invoice in orders.ToList())
            if (!(DateTime.Parse(invoice!.DocDueDate!) > DateTime.Now.AddDays(-7)))
                orders?.Remove(invoice);
        
        var messageText = eLanguage == ELanguage.Uzbek ? "Tugallangan haridlar ✅: \n\n" : "Завершенные ✅: \n\n";

        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
        foreach (var order in orders!.Skip((pageToken - 1) * 10).Take(10))
            inlineKeyboardButtons.Add([
                InlineKeyboardButton.WithCallbackData($"Hujjat raqami №{order!.DocNum} | {DateTime.Parse(order.DocDueDate!):dd.MM.yyyy}",
                    $"completedOrdersDocNum_{order.DocNum}")
            ]);

        InlineKeyboardButton[] inlineKeyBoardArray = new InlineKeyboardButton[orders!.Count / 10 + 1];

        for (int i = 1; i <= orders!.Count / 10 + 1; i++)
            inlineKeyBoardArray[i - 1] = InlineKeyboardButton.WithCallbackData($"{i}", $"completedOrdersOneWeekPage_{i}");

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