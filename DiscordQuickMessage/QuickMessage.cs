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

        // the original embed. this is uses if the user declines the confirmation and we have to resend the original
        public EmbedBuilder Embed { get; private set; }

        // a jump url to the original message. also used for re-building the embed after declining
        public string JumpUrl { get; private set; }

        // the id of the original message. used to differentiate quickmessages since users can respond to multiple
        public ulong MessageId { get; private set; }

        // the three possible responses
        // by default initialize them with the default values
        public string PositiveResponse { get; private set; } = DefaultYes;
        public string NeutralResponse { get; private set; } = DefaultMaybe;
        public string NegativeResponse { get; private set; } = DefaultNo;

        // the confirmation data
        public string ConfirmationResponse { get; private set; } = string.Empty;
        public bool AwaitingConfirmation { get; private set; } = false;


        public QuickMessage(string prompt, EmbedBuilder embed, string jumpUrl, ulong messageId)
        {
            Prompt = prompt; 
            Embed = embed;
            JumpUrl = jumpUrl;
            MessageId = messageId;
        }

        public async Task GenerateResponses()
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

        public void MarkForConfirmation(string response)
        {
            ConfirmationResponse = response;
            AwaitingConfirmation = true;
        }

        public bool IsAwaitingConfirmation => AwaitingConfirmation;

        public void CancelConfirmation()
        {
            ConfirmationResponse = string.Empty;
            AwaitingConfirmation = false;
        }
    }
}
