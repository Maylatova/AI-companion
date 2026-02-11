namespace AI_companion
{
    public static class FileActions
    {
        public static string CreateNewFile(string path, string content)
        {
            try
            {
                if (Path.GetPathRoot(path) == path)
                    return "Вкажіть повний шлях до файлу, а не диск";

                var directory = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(directory))
                    return "Невірний шлях до файлу";

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (File.Exists(path))
                    return "Файл вже існує";

                File.WriteAllText(path, content);
                return "Файл успішно створено";
            }
            catch (Exception ex)
            {
                return $"Помилка створення файлу: {ex.Message}";
            }
        }

        public static string UpdateFile(string path, string content)
        {
            if (!File.Exists(path)) { return "Файл не знайдено"; }

            File.WriteAllText(path, content);
            return "Файл успішно було змінено";
        }

        public static string DeleteFile(string path)
        {
            if (!File.Exists(path))
                return "Файл не знайдено";

            File.Delete(path);
            return "Файл видалено успішно";
        }

        public static string ExecuteCommand(string command)
        {
            var parts = command.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                return "Невірна команда";

            var action = parts[0].ToLower();
            var path = parts[1];

            return action switch
            {
                "create" => parts.Length == 3
                    ? CreateNewFile(path, parts[2])
                    : "Не вказано вміст файлу",

                "update" => parts.Length == 3
                    ? UpdateFile(path, parts[2])
                    : "Не вказано новий вміст файлу",

                "delete" => DeleteFile(path),

                _ => "Невідома команда"
            };
        }
    }
}
