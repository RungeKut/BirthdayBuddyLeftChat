using BirthdayBuddyLeftChat.Services;

namespace BirthdayBuddyLeftChat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Инициализируем настройки
            Settings.Init();
            // Загружаем данные
            DataStorage.Instance.LoadDataAsync();

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            // Запускаем бота
            var botTask = BotClient.Instance.StartAsync(cts.Token);

            // Запускаем ежедневную проверку
            var dailyTask = BirthdayService.Instance.StartDailyCheck(
                sendMessage: BotClient.Instance.SendMessageAsync,
                botClient: BotClient.Instance.botClient!,
                ct: cts.Token);

            try
            {
                await Task.WhenAll(botTask, dailyTask);
            }
            catch (OperationCanceledException)
            {
                // Штатная остановка по Ctrl+C
            }
            Console.WriteLine("Приложение остановлено.");
        }
    }
}
