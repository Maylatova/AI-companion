using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NAudio.Wave;
using Vosk;

namespace AI_companion
{
    // Interface that defines the structure for speech recognition
    public interface IRecognizer
    {
        void StartRecognition(string language, string modelPath, string outputFile); // Method to start recognition
        void StopRecognition(); // Method to stop recognition
    }

    // Main class that implements the IRecognizer interface
    public class SpeechRecognizer : IRecognizer
    {
        private bool _stop = false; // Flag to stop recognition
        private readonly string _language;     // Recognition language (used for error display)
        private readonly string _modelPath;    // Path to Vosk model
        private readonly string _outputFile;   // File for saving recognition results

        // Constructor — initializes language, model path, and output file
        public SpeechRecognizer(string language, string modelPath, string outputFile)
        {
            _language = language;
            _modelPath = modelPath;
            _outputFile = outputFile;
        }

        // Method to start speech recognition
        public void StartRecognition(string language, string modelPath, string outputFile)
        {
            try
            {
                // Load the Vosk model from the specified path
                Model model = new Model(modelPath);

                // Set up the audio recording device (16kHz, mono)
                using var waveIn = new WaveInEvent { WaveFormat = new WaveFormat(16000, 1) };

                // Create the speech recognizer
                using var rec = new VoskRecognizer(model, 16000.0f);

                // Event handler for incoming audio data
                waveIn.DataAvailable += (s, a) =>
                {
                    if (_stop) return; // Stop if the flag is set

                    // If a full phrase is detected
                    if (rec.AcceptWaveform(a.Buffer, a.BytesRecorded))
                    {
                        var result = rec.Result(); // Get the JSON result
                        SaveText(result, outputFile); // Save the recognized text
                    }
                };

                waveIn.StartRecording(); // Start recording from the microphone

                // Wait in a loop until stop flag is set
                while (!_stop)
                {
                    Thread.Sleep(100);
                }

                waveIn.StopRecording(); // Stop the recording
            }
            catch (Exception ex)
            {
                // Print an error message if something goes wrong
                Console.WriteLine($" [{_language}] Error: {ex.Message}");
            }
        }

        // Method to stop recognition — just sets the stop flag
        public void StopRecognition()
        {
            _stop = true;
        }

        // Method to save recognized text to a file
        public static void SaveText(string jsonResult, string filePath)
        {
            // Parse the JSON result and extract the "text" field
            var text = System.Text.Json.JsonDocument.Parse(jsonResult)
                .RootElement.GetProperty("text").GetString();

            // If the text is not empty, save it
            if (!string.IsNullOrWhiteSpace(text))
            {
                // Display the result in the console
                Console.WriteLine($" [{Path.GetFileNameWithoutExtension(filePath)}] → {text}");

                // Append the text to the specified file
                File.AppendAllText(filePath, text + Environment.NewLine);
            }
        }
    }
}
