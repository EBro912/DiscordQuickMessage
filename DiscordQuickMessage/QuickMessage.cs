using Discord;

namespace DiscordQuickMessage
{
    public sealed class QuickMessage
    {
        // Default positive response
        public static readonly string DefaultYes = "Yes";
        // Default negative response
        public static readonly string DefaultNo = "No";
        // Default neutral response
        public static readonly string DefaultMaybe = "Maybe";

        // the original prompt
        public string Prompt { get; private set; }

        // the three possible responses
        // by default initialize them with the default values
        public string PositiveResponse { get; private set; } = DefaultYes;
        public string NeutralResponse { get; private set; } = DefaultMaybe;
        public string NegativeResponse { get; private set; } = DefaultNo;

        // since we cannot use await in constructors, use a create method
        public static async Task<QuickMessage> CreateAsync(string prompt)
        {
            QuickMessage result = new QuickMessage(prompt);
            await result.GenerateResponses();
            return result;
        }

        private QuickMessage(string prompt)
        {
            Prompt = prompt; 
        }

        private async Task GenerateResponses()
        {
            // if any of the responses fail, then the default responses will automatically be the fallback
            try
            {
                PositiveResponse = await OpenAIHandler.GenerateResponse("Give a positive response: " + Prompt);
                NeutralResponse = await OpenAIHandler.GenerateResponse("Give a neutral response: " + Prompt);
                NegativeResponse = await OpenAIHandler.GenerateResponse("Give a negative response: " + Prompt);
            }
            catch
            {
                Console.WriteLine("Failed to generate all three responses for the following prompt:\n" + Prompt);
            }
        }
    }
}
