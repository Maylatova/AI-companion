using AICompanion;
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Список мікрофонів ===");
        MicrophoneManager.ListMicroDevices();

        Console.WriteLine("Введіть індекс мікрофона для вибору:");
        int index = int.Parse(Console.ReadLine());

        string deviceName = MicrophoneManager.GetMicroNameByIndex(index);
        Console.WriteLine($"Вибрано пристрій: {deviceName}");

        MicrophoneWriteToCFG.SaveMicrophone(index, deviceName);
    }
}