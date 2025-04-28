using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace AI_companion
{
    // Class for managing speech recognition in Ukrainian and English languages
    public class SpeechRecognizerManager
    {
        private SpeechRecognitionEngine _recognizerUkrainian; // Object for Ukrainian speech recognition
        private SpeechRecognitionEngine _recognizerEnglish;   // Object for English speech recognition
        private readonly string _outputFilePath;              // Path to file for saving recognition results

        // Class constructor, initializes file path and recognizers
        public SpeechRecognizerManager(string outputFilePath = "transcription.txt")
        {
            _outputFilePath = outputFilePath;

            InitializeRecognizers(); // Initialize speech recognizers
        }

        // Method for initializing speech recognizers
        private void InitializeRecognizers()
        {
            _recognizerUkrainian = new SpeechRecognitionEngine(new CultureInfo("uk-UA")); // Ukrainian speech recognizer
            _recognizerEnglish = new SpeechRecognitionEngine(new CultureInfo("en-US"));   // English speech recognizer

            _recognizerUkrainian.SetInputToDefaultAudioDevice(); // Setting up a standard input device (microphone)
            _recognizerEnglish.SetInputToDefaultAudioDevice();

            _recognizerUkrainian.LoadGrammar(new DictationGrammar()); // Downloading dictation grammar
            _recognizerEnglish.LoadGrammar(new DictationGrammar());

            _recognizerUkrainian.SpeechRecognized += Recognizer_SpeechRecognized; // Subscribe to speech recognition event
            _recognizerEnglish.SpeechRecognized += Recognizer_SpeechRecognized;
        }

        // Method to start the speech recognition process
        public void StartRecognition()
        {
            _recognizerUkrainian.RecognizeAsync(RecognizeMode.Multiple); // Asynchronous multiple recognition
            _recognizerEnglish.RecognizeAsync(RecognizeMode.Multiple);
        }

        // Method to stop speech recognition process
        public void StopRecognition()
        {
            _recognizerUkrainian.RecognizeAsyncStop(); // Stopping asynchronous recognition
            _recognizerEnglish.RecognizeAsyncStop();
        }

        // Successful speech recognition event handler
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.7) // Checking the recognition confidence level (the threshold can be adjusted)
            {
                string recognizedText = e.Result.Text;
                File.AppendAllText(_outputFilePath, recognizedText + Environment.NewLine); // Saving recognized text to file
            }
        }
    }
}
