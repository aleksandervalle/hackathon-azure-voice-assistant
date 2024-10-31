using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace AzureVoiceAssistant
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Azure Voice Assistant Console Application");
            Console.WriteLine("=========================================");
            Console.WriteLine("Instructions:");
            Console.WriteLine("1. Press 'R' to start recording.");
            Console.WriteLine("2. Press 'S' to stop recording.");
            Console.WriteLine("3. The recorded audio will be transcribed and processed.");
            Console.WriteLine("4. Type 'exit' to quit the application.");
            Console.WriteLine();

            var conversationManager = new ConversationManager();
            var asrService = new AsrService();
            var ttsService = new TtsService();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.R)
                {
                    Console.WriteLine("Recording started. Speak into your microphone...");
                    string transcribedText = await asrService.RecognizeSpeechAsync();
                    if (!string.IsNullOrEmpty(transcribedText))
                    {
                        Console.WriteLine($"You said: {transcribedText}");
                        conversationManager.AddMessage("user", transcribedText);

                        string prompt = conversationManager.GetConversationPrompt();
                        string botResponse = await GenerateBotResponseAsync(prompt, "asdf");

                        conversationManager.AddMessage("bot", botResponse);
                        Console.WriteLine($"Bot: {botResponse}");

                        await ttsService.SynthesizeAndPlayAsync(botResponse);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape || (keyInfo.KeyChar == 'e' || keyInfo.KeyChar == 'E'))
                {
                    Console.WriteLine("Exiting application...");
                    break;
                }
            }
        }

        static async Task<string> GenerateBotResponseAsync(string prompt, string region)
        {
            return "This is a placeholder response. Implement your own logic here.";

            string apiEndpoint = $"https://{region}.api.cognitive.microsoft.com/your-conversational-model-endpoint";
            string apiKey = Environment.GetEnvironmentVariable("CONVERSATION_API_KEY"); // Set this environment variable

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            var payload = new
            {
                prompt = prompt,
                max_tokens = 250
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(jsonResponse);
            return result.generated_text;
        }
    }
}
