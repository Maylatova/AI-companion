using System;
using System.IO;
using NAudio.Wave;

namespace AICompanion
{
    public static class MicrophoneManager
    {
        public static void ListMicroDevices()
        {
            int waveInDevices = WaveInEvent.DeviceCount;
            for (int i = 0; i < waveInDevices; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                Console.WriteLine($"Індекс - {i}, Назва мікрофону: {deviceInfo.ProductName}");
            }
        }
        public static string GetMicroNameByIndex(int index)
        {
            if (index < 0 || index >= WaveInEvent.DeviceCount)
                throw new ArgumentOutOfRangeException(nameof(index), "Невірний індекс пристрою.");

            var deviceInfo = WaveInEvent.GetCapabilities(index);
            return deviceInfo.ProductName;
        }
    }
    public static class MicrophoneWriteToCFG
    {
        private static readonly string cfgFilePath = "D:\\AIComp\\user.cfg";
        public static void SaveMicrophone(int MicroID, string MicroName)
        {
            try
            {
                using StreamWriter writer = new(cfgFilePath, false);
                writer.WriteLine($"MicroID = {MicroID}");
                writer.WriteLine($"MicroName = {MicroName}");
            }
            catch (Exception er)
            {
                Console.WriteLine($"Виникла помилка при записі до конфігураційного файлу: {er.Message}");
            }
        }
    }
}
