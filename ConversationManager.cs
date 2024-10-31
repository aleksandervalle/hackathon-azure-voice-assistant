using System.Collections.Generic;
using System.Text;

namespace AzureVoiceAssistant
{
    public class ConversationManager
    {
        private List<(string Sender, string Message)> conversation;

        public ConversationManager()
        {
            conversation = new List<(string, string)>
            {
                ("user", "You are a large language model known as OpenChat, the open-source counterpart to ChatGPT, equally powerful as its closed-source sibling. You communicate using an advanced deep learning based speech synthesis system made by coqui, so feel free to include interjections (such as 'hmm', 'oh', 'right', 'wow'...), but avoid using emojis, symbols, code snippets, or anything else that does not translate well to spoken language. For example, instead of %, say percent; instead of =, say equal; and for *, say times, etc. Also please avoid using lists with numbers as items like so 1. 2. Use regular sentences instead."),
                ("bot", "No problem. Anything else?"),
                ("user", "Yeah, please always respond in a sentence or two from now on."),
                ("bot", "Sure, I'll be concise.")
            };
        }

        public void AddMessage(string sender, string message)
        {
            conversation.Add((sender, message));
        }

        public string GetConversationPrompt()
        {
            StringBuilder prompt = new StringBuilder();
            foreach (var (Sender, Message) in conversation)
            {
                if (Sender == "user")
                {
                    prompt.AppendLine($"User: {Message}");
                    prompt.Append("Assistant:");
                }
                else
                {
                    prompt.AppendLine($"{Message}");
                }
            }
            return prompt.ToString();
        }
    }
}
