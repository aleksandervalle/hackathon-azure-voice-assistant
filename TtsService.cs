using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;

namespace AzureVoiceAssistant
{
    public class TtsService
    {
        private readonly SpeechConfig speechConfig;

        public TtsService()
        {
            string speechKey = AppConfig.SpeechKey;
            string speechRegion = AppConfig.SpeechRegion;

            if (string.IsNullOrEmpty(speechKey) || string.IsNullOrEmpty(speechRegion))
            {
                throw new Exception("Please set the SPEECH_KEY and SPEECH_REGION environment variables.");
            }

            speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechSynthesisVoiceName = "en-US-AvaNeural"; // Set desired voice
        }

        public async Task SynthesizeAndPlayAsync(string text)
            {
                using var synthesizer = new SpeechSynthesizer(speechConfig);

                var result = await synthesizer.SpeakTextAsync(text);

                switch (result.Reason)
                {
                    case ResultReason.SynthesizingAudioCompleted:
                        Console.WriteLine($"Speech synthesized for text: [{text}]");
                        break;

                    case ResultReason.Canceled:
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            Console.WriteLine("CANCELED: Did you set the speech resource key and region values?");
                        }
                        break;

                    default:
                        break;
                }
            }
    }
}
