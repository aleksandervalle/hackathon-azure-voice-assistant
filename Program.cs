using System;
using System.ClientModel;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

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

            AzureOpenAIClient azureClient = new(
                new Uri(AppConfig.ChatCompletionEndpoint),
                new ApiKeyCredential(AppConfig.ChatCompletionKey));

            ChatClient chatClient = azureClient.GetChatClient(AppConfig.ChatCompletionModel);

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
                        conversationManager.AddUserMessage(transcribedText);

                        string botResponse = await GenerateBotResponseAsync(conversationManager, chatClient);

                        if (!string.IsNullOrEmpty(botResponse))
                        {
                            conversationManager.AddAssistantMessage(botResponse);
                            Console.WriteLine($"Bot: {botResponse}");

                            await ttsService.SynthesizeAndPlayAsync(botResponse);
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape || (keyInfo.KeyChar == 'e' || keyInfo.KeyChar == 'E'))
                {
                    Console.WriteLine("Exiting application...");
                    break;
                }
            }
        }

        static async Task<string> GenerateBotResponseAsync(ConversationManager conversationManager, ChatClient chatClient)
        {
            var conversation = conversationManager.GetConversation();

            // Call the Azure OpenAI Chat API with the conversation history
            var response = await chatClient.CompleteChatAsync(
                conversation,
                new ChatCompletionOptions
                {
                    Temperature = 0.0f
                });

            return response?.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty;
        }
    }
}
