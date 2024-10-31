using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace AzureVoiceAssistant
{
    public class AsrService
    {
        private readonly SpeechConfig speechConfig;

        public AsrService()
        {
            string speechKey = AppConfig.SpeechKey;
            string speechRegion = AppConfig.SpeechRegion;

            if (string.IsNullOrEmpty(speechKey) || string.IsNullOrEmpty(speechRegion))
            {
                throw new Exception("Please set the SPEECH_KEY and SPEECH_REGION environment variables.");
            }

            speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US"; // Set desired language
        }

        public async Task<string> RecognizeSpeechAsync()
            {
                using var recognizer = new SpeechRecognizer(speechConfig);

                var result = await recognizer.RecognizeOnceAsync();

                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        return result.Text;

                    case ResultReason.NoMatch:
                        Console.WriteLine("No speech could be recognized.");
                        return string.Empty;

                    case ResultReason.Canceled:
                        Console.WriteLine("CANCELED");
                        return string.Empty;

                    default:
                        return string.Empty;
                }
            }
    }
}
