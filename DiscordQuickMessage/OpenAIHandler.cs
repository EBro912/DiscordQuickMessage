using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace DiscordQuickMessage
{
    public class OpenAIHandler
    {
        private static OpenAIAPI openAI;

        public static void Init()
        {
            if (!File.Exists("openai_token.txt"))
            {
                throw new FileNotFoundException("Could not find openai_token.txt in the current directory.");
            }
            string token = File.ReadAllText("openai_token.txt");
            openAI = new OpenAIAPI(token);
            Console.WriteLine("[OpenAIHandler]: OpenAIHandler initalized successfully.");
        }

        public static async Task<string> GenerateResponse(string input)
        {
            if (openAI == null)
            {
                Console.WriteLine("[OpenAIHandler]: OpenAIAPI object is null. Cannot perform any OpenAI actions.");
                return string.Empty;
            }

            var result = await openAI.Completions.CreateCompletionAsync(new CompletionRequest(input, temperature: 0.5, model: Model.DavinciText, max_tokens: 256));
            return result.Completions.First().Text;
        }

    }
}
