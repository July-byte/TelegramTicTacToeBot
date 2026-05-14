// код для телеграм бота игры в кретсики нолики

using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    static async Task Main()
    {
      string token = "<<TGTOKEN>>";
      var botClient = new TelegramBotClient(token);
      using var cts = new CancellationTokenSourse();
      var receiverOptions = new ReceiverOptions
      {
        AllowedUpdates = Array.Empty<UpdateType>()
        };

      botClient.StartReceiving(
        HandleUpdateAsync,
        HandleErrorAsync,
        receiverOptions,
        cancellationToken: cts.Token
        );

      var me = await bot.Client.GetMeAsync();
      Console.WriteLine($"Бот @{me.Username} запущен...");
      Console.ReadLine();
    }

  static async Task HandleUpdateAsync(
    ITelegramBotClient botClient,
    Update update,
    CancellationToken cancellationToken)
  {
    if (update.Type != Update.Message)
      return;
    if (update.Message!.Type != MessageType.Text)
      return;
    var chatId = updateMessage.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Получено сообщение: {message.Text}");
    if (message.Text == "/start")
    {
      await botClient.SendTextMessageAsync(
        chatId,
        "Привет!\nЭто бот для игры в Крестики-Нолики.\nНапиши /play чтобы начать игру.",
        cancellationToken: cancellationToken);
    }
    if (messageText == "/play")
    {
      await botClient.SendTextMessageAsync(
        chatId, "Игра скоро начнется",
        cancellationToken : cancellationToken);
    }
  }
  static Task HandleErrorAsync(
    ITelegramBotClient botClient,
    Exception exception,
    CancellationToken cancellationToken)
  {
    Console.WriteLine($"Ошибка: {exception.Message}");
    return Task.Completed.Task;
  }
}
