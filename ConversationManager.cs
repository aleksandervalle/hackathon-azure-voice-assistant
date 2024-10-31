using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;

namespace AzureVoiceAssistant
{
    public class ConversationManager
    {
        private List<ChatMessage> conversation;

        public ConversationManager()
        {
            conversation = new List<ChatMessage>
            {
                new SystemChatMessage("Du er en hjelpsom assistent som gir korte svar.")
            };
        }

        public void AddUserMessage(string message)
        {
            conversation.Add(new UserChatMessage(message));
        }

        public void AddAssistantMessage(string message)
        {
            conversation.Add(new AssistantChatMessage(message));
        }

        public IReadOnlyList<ChatMessage> GetConversation()
        {
            return conversation.AsReadOnly();
        }
    }
}
