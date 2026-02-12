using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace AI_companion
{
    public static class ModelManager
    {
        private static readonly string BaseModelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vosk", "models");

        public static async Task<string> EnsureModelExists(string modelName, string downloadUrl)
        {
            string modelDirectory = Path.Combine(BaseModelsPath, modelName);

            if (Directory.Exists(modelDirectory))
            {
                Console.WriteLine($"[ModelManager] Модель {modelName} знайдена.");
                return modelDirectory;
            }

            Directory.CreateDirectory(BaseModelsPath);

            string zipPath = Path.Combine(BaseModelsPath, $"{modelName}.zip");

            Console.WriteLine($"[ModelManager] Модель {modelName} не знайдена. Починаємо завантаження...");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(downloadUrl);
                response.EnsureSuccessStatusCode();

                using (var fs = new FileStream(zipPath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            Console.WriteLine($"[ModelManager] Завантаження завершено. Розпакування...");

            ZipFile.ExtractToDirectory(zipPath, BaseModelsPath);

            File.Delete(zipPath);

            Console.WriteLine($"[ModelManager] Модель {modelName} готова до роботи.");
            return modelDirectory;
        }
    }
}