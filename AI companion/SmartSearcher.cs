using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace AI_companion
{
    public class SmartSearcher
    {
        private const string ApiKey = "sk-8625df1a3f7c4c9497e1ca5c15dc01bc";
        private const string ApiUrl = "https://api.deepseek.com/v1/chat/completions";

        public async Task<string> GetActionFromAi(string userText)
        {
            string prompt = $@"Ти — системний помічник. Проаналізуй фразу: '{userText}'.
            Перетвори її на ОДИН з цих варіантів (і більше нічого не пиши):
            - create [назва_файлу] [вміст]
            - update [назва_файлу] [новий_вміст]
            - delete [назва_файлу]
            - search [запит] (якщо користувач хоче знайти щось в інтернеті)
            - NONE (якщо команда незрозуміла)
    
            Пиши ТІЛЬКИ технічну команду.";

            string aiResponse = await GetAiResponse(prompt);

            return aiResponse;
        }

        private async Task<string> GetAiResponse(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[] { new { role = "user", content = prompt } },
                temperature = 0.3
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ApiUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString().Trim();
        }

        private void OpenBrowser(string query)
        {
            string url = $"https://www.google.com/search?q={Uri.EscapeDataString(query)}";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                Console.WriteLine($"[SmartSearch] Відкрито браузер для запиту: {query}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка] Не вдалося відкрити браузер: {ex.Message}");
            }
        }
    }
}