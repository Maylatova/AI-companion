using System.Text;
using System.Text.Json;

namespace AI_companion
{
    internal class DeepSeekClient
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey;

        public DeepSeekClient()
        {
            apiKey = "sk-d3ee068a81c4433c90d996c4e048f90e";

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> SendPromptAsync(string prompt)
        {
            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.deepseek.com/v1/chat/completions",
                content
            );

            if (!response.IsSuccessStatusCode)
                return $"API error: {response.StatusCode}";

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            return doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
