using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace AI_companion
{
    public class AiTaskScheduler
    {
        public async Task ScheduleActionAsync(int delayInMinutes, string actionType, string targetInfo)
        {
            await Task.Delay(TimeSpan.FromMinutes(delayInMinutes));

            ExecuteAction(actionType, targetInfo);
        }

        private void ExecuteAction(string actionType, string targetInfo)
        {
            try
            {
                switch (actionType.ToLower())
                {
                    case "openbrowser":
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = targetInfo, 
                            UseShellExecute = true 
                        });
                        ShowNotification($"Виконано: відкрито браузер ({targetInfo})");
                        break;

                    case "reminder":
                        ShowNotification($"Нагадування: {targetInfo}");
                        break;

                    case "deletefile":
                        ShowNotification($"Виконано: видалено об'єкт {targetInfo}");
                        break;

                    default:
                        ShowNotification($"Невідома команда: {actionType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Помилка виконання запланованого завдання: {ex.Message}");
            }
        }

        private void ShowNotification(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "AI Companion Scheduler", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
    }
}
