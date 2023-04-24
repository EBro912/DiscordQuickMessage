using Discord;
using Discord.Interactions;

namespace DiscordQuickMessage
{
    // This class is where all slash command logic goes

    public class Commands : InteractionModuleBase<SocketInteractionContext>
    {
        // a simple command which responds with the word "Pong!"
        // here we use the [SlashCommand] attribute to designate this function as a slash command with the name "ping" and the description "Pong!"
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/
        [SlashCommand("ping", "Pong!")]
        public async Task Ping()
        {
            // we can respond to the user with this function
            // this will also "acknowledge" Discord that we received the command
            await RespondAsync("Pong!");
        }

        [SlashCommand("dnd", "Toggles Do Not Disturb mode")]
        public async Task DoNotDisturb()
        {
            // ephemeral sends the message privately to the person who ran the command

            if (QuickMessageHandler.ToggleDoNotDisturb(Context.User.Id))
            {
                await RespondAsync("You have **enabled** Do Not Disturb mode.\n__You will not receive QuickMessage message when mentioned.__", ephemeral: true);
            }
            else
            {
                await RespondAsync("You have **disabled** Do Not Disturb mode.\n__You will now receive QuickMessage message when mentioned.__", ephemeral: true);
            }
        }
    }
}
