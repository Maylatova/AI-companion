using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using Vosk;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace AI_companion
{
    public interface IRecognizer
    {
        Task Initialize(string language); 
        void StartRecognition(string language, string outputFile); 
        void StopRecognition(); 
    }

    public class SpeechRecognizer : IRecognizer
    {
        private bool _stop = false;
        private string _modelPath;

        private const string UkModelUrl = "https://alphacephei.com/vosk/models/vosk-model-small-uk-v3-nano.zip";
        private const string EnModelUrl = "https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip";

        public async Task Initialize(string language)
        {
            string modelName = language.ToLower() == "uk" ? "vosk-model-small-uk-v3-nano" : "vosk-model-small-en-us-0.15";
            string url = language.ToLower() == "uk" ? UkModelUrl : EnModelUrl;

            string baseModelsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vosk", "models");
            _modelPath = Path.Combine(baseModelsDir, modelName);

            if (Directory.Exists(_modelPath))
            {
                Console.WriteLine($"[Vosk] Модель для мови '{language}' вже готова.");
                return;
            }

            Directory.CreateDirectory(baseModelsDir);
            string zipPath = Path.Combine(baseModelsDir, modelName + ".zip");

            Console.WriteLine($"[Vosk] Завантаження моделі ({language})...");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var fs = new FileStream(zipPath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            Console.WriteLine($"[Vosk] Розпакування моделі...");
            ZipFile.ExtractToDirectory(zipPath, baseModelsDir);

            if (File.Exists(zipPath)) File.Delete(zipPath);

            Console.WriteLine($"[Vosk] Модель успішно встановлена у: {_modelPath}");
        }

        public void StartRecognition(string language, string outputFile)
        {
            _stop = false;

            if (string.IsNullOrEmpty(_modelPath) || !Directory.Exists(_modelPath))
            {
                Console.WriteLine(" [Помилка] Модель не знайдена. Спочатку викличте метод Initialize().");
                return;
            }

            try
            {
                Model model = new Model(_modelPath);

                using var waveIn = new WaveInEvent { WaveFormat = new WaveFormat(16000, 1) };

                using var rec = new VoskRecognizer(model, 16000.0f);

                waveIn.DataAvailable += (s, a) =>
                {
                    if (_stop) return;

                    if (rec.AcceptWaveform(a.Buffer, a.BytesRecorded))
                    {
                        var result = rec.Result();
                        SaveText(result, outputFile, language);
                    }
                };

                waveIn.StartRecording();
                Console.WriteLine($"[Vosk] Слухаю ({language}). Натисніть StopRecognition для завершення...");

                while (!_stop)
                {
                    Thread.Sleep(100);
                }

                waveIn.StopRecording();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [Помилка] Під час розпізнавання: {ex.Message}");
            }
        }

        public void StopRecognition()
        {
            _stop = true;
        }

        private static void SaveText(string jsonResult, string filePath, string lang)
        {
            using var jsonDoc = JsonDocument.Parse(jsonResult);
            var text = jsonDoc.RootElement.GetProperty("text").GetString();

            if (!string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine($" [{lang.ToUpper()}] → {text}");
                File.AppendAllText(filePath, $"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            }
        }
    }
}
