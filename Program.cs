// код шаблон для телеграм бота игры в крестики нолики

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static Dictionary<long, Game> games = new();

    static async Task Main()
    {
        string token = "tg-token"; 

        var botClient = new TelegramBotClient(token);

        using var cts = new CancellationTokenSource();

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

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Бот @{me.Username} запущен...");
        Console.ReadLine();
    }

    static async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        // обработка сообщений
        if (update.Type == UpdateType.Message &&
            update.Message!.Type == MessageType.Text)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Сообщение: {messageText}");

            if (messageText == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Привет 👋\nЭто бот для игры в Крестики‑Нолики.\nНапиши /play чтобы начать игру.",
                    cancellationToken: cancellationToken);
            }

            if (messageText == "/play")
            {
                var game = new Game();
                games[chatId] = game;

                await botClient.SendTextMessageAsync(
                    chatId,
                    "Игра началась! Ты играешь за X",
                    replyMarkup: GetGameKeyboard(game),
                    cancellationToken: cancellationToken);
            }
        }

        // обработка нажатий кнопок 
        if (update.Type == UpdateType.CallbackQuery)
        {
            var callback = update.CallbackQuery!;
            var chatId = callback.Message!.Chat.Id;

            if (!games.ContainsKey(chatId))
                return;

            var game = games[chatId];

            var parts = callback.Data!.Split(',');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);

            // ход игрока
            if (!game.MakeMove(row, col, 'X'))
                return;

            // проверка победы игрока
            if (game.CheckWin('X'))
            {
                await botClient.EditMessageTextAsync(
                    chatId,
                    callback.Message.MessageId,
                    "Ты победил!",
                    replyMarkup: GetGameKeyboard(game),
                    cancellationToken: cancellationToken);

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

            // ход бота
            game.MakeBotMove();

            if (game.CheckWin('O'))
            {
                await botClient.EditMessageTextAsync(
                    chatId,
                    callback.Message.MessageId,
                    "Бот победил",
                    replyMarkup: GetGameKeyboard(game),
                    cancellationToken: cancellationToken);

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

            // обновляем поле
            await botClient.EditMessageReplyMarkupAsync(
                chatId,
                callback.Message.MessageId,
                replyMarkup: GetGameKeyboard(game),
                cancellationToken: cancellationToken);
        }
    }

    static InlineKeyboardMarkup GetGameKeyboard(Game game)
    {
        var buttons = new List<InlineKeyboardButton[]>();

        for (int i = 0; i < 3; i++)
        {
            var row = new List<InlineKeyboardButton>();

            for (int j = 0; j < 3; j++)
            {
                string text = game.Board[i, j] == ' '
                    ? "⬜"
                    : game.Board[i, j].ToString();

                string callbackData = $"{i},{j}";

                row.Add(InlineKeyboardButton.WithCallbackData(text, callbackData));
            }

            buttons.Add(row.ToArray());
        }

        return new InlineKeyboardMarkup(buttons);
    }

    static Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}
