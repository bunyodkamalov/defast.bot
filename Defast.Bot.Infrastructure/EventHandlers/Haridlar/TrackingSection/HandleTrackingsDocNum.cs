using System.Globalization;
using System.Text;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar.TrackingSection;

public class HandleTrackingsDocNum(ITrackingService trackingService)
{
    public async ValueTask Handle(int docNum, ITelegramBotClient tgBotClient, Message message, ELanguage eLanguage, CancellationToken cancellationToken)
    {
        var tracking = await trackingService.GetByDocNumAsync(docNum.ToString(), cancellationToken);
        
        if (tracking is not null)
        {
            var messageText = new StringBuilder(eLanguage == ELanguage.Uzbek
                ? "Tracking: \n\n"
                : "Трекинг: \n\n");

            messageText.Append(eLanguage == ELanguage.Uzbek
                ? $"Hujjat raqami: {tracking.DocNum ?? ""}\n" +
                  $"Mijoz ismi: {tracking.CardName ?? ""} \n" +
                  $"Konteyner №: {tracking.U_numberOfCntr ?? ""}\n" +
                  $"China platform: {tracking.U_China_platform ?? ""}\n" +
                  $"Platforma KZX №: {tracking.U_numberPlatformKzx ?? ""}\n" +
                  $"Temir yo'l stansiyasi: {tracking.U_StationOfOperationRailway ?? ""}\n" +
                  $"Operatsiya sanasi: {(tracking.U_DateOfOperation is not null ? DateTime.ParseExact(tracking.U_DateOfOperation!,"yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy") : "")}\n" +
                  $"Operatsiya(liniya): {tracking.U_LineOfOperation ?? ""}\n" +
                  $"Stansiya manzili: {tracking.U_DestinationStation ?? ""}\n" +
                  $"Qolgan masofa(km): {tracking.U_Remaining_km ?? 0}\n" +
                  $"Taxminiy etkazib berish muddati: {(tracking.U_DispatchPlan is not null ? DateTime.ParseExact(tracking.U_DispatchPlan, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy") : "")}\n" +
                  $"Jo'nash sanasi: {(tracking.U_DateSending is not null ? DateTime.ParseExact(tracking.U_DateSending!, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy") : "")}\n"

                : $"Номер документа: {tracking.DocNum ??  ""}\n" +
                  $"Имя клиента: {tracking.CardName ?? ""} \n" +
                  $"Контейнер №: {tracking.U_numberOfCntr ?? ""}\n" +
                  $"China platform: {tracking.U_China_platform ?? ""}\n" +
                  $"Platforma KZX №: {tracking.U_numberPlatformKzx ?? ""}\n" +
                  $"Станция операции ж.д: {tracking.U_StationOfOperationRailway ?? ""}\n" +
                  $"Дата операции: {(tracking.U_DateOfOperation is not null ? DateTime.ParseExact(tracking.U_DateOfOperation!,"yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy") : "")}\n" +
                  $"Операция(линия): {tracking.U_LineOfOperation ?? ""}\n" +
                  $"Станция назначения: {tracking.U_DestinationStation ?? ""}\n" +
                  $"Остаточное расстояние, км: {tracking.U_Remaining_km ?? 0}\n" +
                  $"Расчетный срок доставки: {(tracking.U_DispatchPlan is not null ? DateTime.ParseExact(tracking.U_DispatchPlan, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy") : "")}\n" +
                  $"Дата отправки: {(tracking.U_DateSending is not null ? DateTime.ParseExact(tracking.U_DateSending, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy") : "")}\n");

            await tgBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId, cancellationToken);

            await tgBotClient.SendTextMessageAsync(
                message.Chat.Id,
                messageText.ToString(),
                replyMarkup: UserMainMenuMarkup.Get(eLanguage),
                cancellationToken: cancellationToken);
        }
        
    }
}