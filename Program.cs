// код для телеграм бота игры в крестики нолики

using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    using Telegram.Bot.Types.ReplyMarkups;

static InlineKeyboardMarkup GetGameKeyboard(Game game)
{
    var buttons = new List<InlineKeyboardButton[]>();

    for (int i = 0; i < 3; i++)
    {
        var row = new List<InlineKeyboardButton>();

        for (int j = 0; j < 3; j++)
        {
            string text = game.Board[i, j] == ' ' ? "⬜" : game.Board[i, j].ToString();
            string callbackData = $"{i},{j}";

            row.Add(InlineKeyboardButton.WithCallbackData(text, callbackData));
        }

        buttons.Add(row.ToArray());
    }

    return new InlineKeyboardMarkup(buttons);
}
    static Dictionary<lomg, Game> games = new();
    
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
        var game = new Game();
        games[chatId] = game;
        
      await botClient.SendTextMessageAsync(
        chatId, "Игра началась! Ты играешь за Х",
        replyMarkup: GetGameKeyBoard(game),
        cancellationToken : cancellationToken);
    }

    if (update.Type == UpdateType.CallbackQuery)
    {
        var callback = update.CallbackQuery!;
        var chatId = callback.Message!.Chat.Id;

        if (!games.ContainsKey(chatId))
            return;
        var game = games[chatId];
        var parts = callback.Data.Split('.');
        int row = int.Parse(parts[0]);
        int col = int.Parse(parts[1]);
        if (!game.MakeMove(row,col,'X'))
            return;
        if (game.CheckWin('X'))
        {
            await botClient.EditMessageTextAsync(
                chatId,
                callback.Message.MessageId,
                "Ты победил!",
                replyMarkup: GetGameKeyboard(game),
                cancellationToken: cancellationTokem);

            games.Remove(chatId);
            return;
        }

        if (game.IsBoardFull())
        {
            await botClient.EditMessageTextAsync(
                chatId,
                callback.Message.MessageId,
                "Ничья",
                replyMarkup: GetGameKeyboard(game),
                cancellationToken: cancellationToken);

            games.Remove(chatId);
            return;
        }
        await botClient.EditMessageReplyMarkupAsync(
            chatId,
            callback.Message.MessageId,
            replyMarkup: GetGameKeyboard(game),
            cancellationToken: cancellationToken);
        
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
