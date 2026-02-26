using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AI_companion
{
    public class CommandProcessor
    {
        private readonly SmartSearcher _aiManager;

        public CommandProcessor()
        {
            _aiManager = new SmartSearcher();
        }

        public async Task ProcessSpeech(string recognizedText)
        {
            if (string.IsNullOrWhiteSpace(recognizedText)) return;

            Console.WriteLine($"[Голос]: {recognizedText}");

            string aiResponse = await _aiManager.GetActionFromAi(recognizedText);

            if (aiResponse == "NONE")
            {
                Console.WriteLine("[ШІ]: Команду не розпізнано або вона не стосується файлів чи пошуку.");
                return;
            }

            if (aiResponse.StartsWith("search"))
            {
                string query = aiResponse.Replace("search ", "").Trim();
                OpenWebSearch(query);
            }

            else
            {
                string result = FileActions.ExecuteCommand(aiResponse);
                Console.WriteLine($"[Система]: {result}");
            }
        }

        private void OpenWebSearch(string query)
        {
            try
            {
                string url = $"https://www.google.com/search?q={Uri.EscapeDataString(query)}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                Console.WriteLine($"[Браузер]: Відкрито пошук для: '{query}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка браузера]: {ex.Message}");
            }
        }
    }
}