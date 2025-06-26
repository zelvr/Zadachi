using Microsoft.Extensions.Options;
using Telegram.Bot;
using Zadachi.Lib.Telegram;

public class TelegramNotifier
{
    private readonly TelegramSettings _settings;

    public TelegramNotifier(IOptions<TelegramSettings> options)
    {
        _settings = options.Value;

        // Валидация настроек
        if (string.IsNullOrEmpty(_settings.BotToken))
            throw new ArgumentNullException(nameof(_settings.BotToken));

        if (_settings.ChatId == 0)
            throw new ArgumentNullException(nameof(_settings.ChatId));
    }

    public async Task SendNotificationAsync(string message)
    {
        var botClient = new TelegramBotClient(_settings.BotToken);
        await botClient.SendMessage(_settings.ChatId, message);
    }
}
