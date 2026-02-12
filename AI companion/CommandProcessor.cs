using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_companion
{
    public class CommandProcessor
    {
        private readonly SpeechRecognizer _recognizer;
        private readonly string _logFile = "commands_log.txt";

        public CommandProcessor()
        {
            _recognizer = new SpeechRecognizer();
        }

        public void Start(string language, string modelPath)
        {
            Task.Run(() => _recognizer.StartRecognition(language, modelPath, _logFile));

            WatchForCommands();
        }

        private void WatchForCommands()
        {
            Console.WriteLine("ШІ працює... Очікування голосових команд.");
            long lastSize = 0;

            while (true)
            {
                if (File.Exists(_logFile))
                {
                    var info = new FileInfo(_logFile);
                    if (info.Length > lastSize)
                    {
                        string lastLine = File.ReadLines(_logFile).LastOrDefault();

                        if (!string.IsNullOrEmpty(lastLine))
                        {
                            string commandText = lastLine.ToLower();
                            ProcessVoiceCommand(commandText);
                        }
                        lastSize = info.Length;
                    }
                }
                Thread.Sleep(500); 
            }
        }

        private void ProcessVoiceCommand(string text)
        {
            if (text.Contains("видалити папку") || text.Contains("delete folder"))
            {
                string folderName = text.Split(' ').Last(); 
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"[AI] Папку '{folderName}' успішно видалено.");
                }
            }
            else if (text.Contains("create") || text.Contains("створити"))
            {
                string formattedCmd = text.Replace("створити файл", "create").Replace("create file", "create").Trim();
                string result = FileActions.ExecuteCommand(formattedCmd);
                Console.WriteLine($"[FileActions]: {result}");
            }
            else if (text.Contains("delete") || text.Contains("видалити"))
            {
                string formattedCmd = text.Replace("видалити файл", "delete").Replace("delete file", "delete").Trim();
                string result = FileActions.ExecuteCommand(formattedCmd);
                Console.WriteLine($"[FileActions]: {result}");
            }
        }
    }
}