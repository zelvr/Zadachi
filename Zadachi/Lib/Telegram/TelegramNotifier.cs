using Microsoft.Extensions.Options;
using Telegram.Bot;
using Zadachi.Lib.Telegram;

public class TelegramNotifier
{
    private readonly TelegramSettings _settings;

    public TelegramNotifier(IConfiguration config)
    {
        _settings = new TelegramSettings
        {            
            BotToken = ReadSecret(config, "TelegramSettings:BotToken"),
            ChatId = long.Parse(ReadSecret(config, "TelegramSettings:ChatId"))
        };

        ValidateSettings();
    }

    private string ReadSecret(IConfiguration config, string key)
    {
        var filePath = config[$"{key}_FILE"];
        return File.Exists(filePath)
            ? File.ReadAllText(filePath).Trim()
            : config[key];
    }

    private void ValidateSettings()
    {
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
