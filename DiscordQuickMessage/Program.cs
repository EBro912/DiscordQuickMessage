/* 
 *          Discord Quick Message
 * 
 *     Software Engineering II Project
 * 
 * Created By:
 *  Ethan Derr
 *  Ryan Casebolt
 *  Vincent Gordon
 *  Sandra Hawkins
 *  Anson Cheang
 * 
 * 
 * Libraries Used:
 *  Discord.NET
 *  Microsoft.Extensions.DependencyInjection
 * 
 */


using Discord;
using Discord.WebSocket;
using System.Reflection;
using Discord.Interactions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordQuickMessage
{
    internal class Program
    {
        // temporarily disable this nullable warning since these three variables will never be null
        #pragma warning disable CS8618
        private DiscordSocketClient client;
        private InteractionService interaction;
        private IServiceProvider services;
        #pragma warning restore CS8618
        private bool registerSlashCommands = false;
        private readonly IEmote oneEmoji = Emoji.Parse(":one:");
        private readonly IEmote twoEmoji = Emoji.Parse(":two:");
        private readonly IEmote threeEmoji = Emoji.Parse(":three:");

        // the main function only calls the start function and waits for it to complete
        public static void Main(string[] args)
            => new Program().Start(args).GetAwaiter().GetResult();

        // register all bot related events
        public async Task RegisterEvents()
        {
            // call the Log function whenever something needs to be logged to the console
            client.Log += Log;
            // call the Ready function whenever the bot has successfully connected to Discord
            client.Ready += Ready;

            // lambda function for whenever a user runs a slash command in Discord
            client.InteractionCreated += async (x) =>
            {
                // get the "context" of the command and run the corresponding command given the context
                SocketInteractionContext context = new(client, x);
                await interaction.ExecuteCommandAsync(context, services);
            };

            // lambda function for whenever the bot receives a message
            client.MessageReceived += async (x) =>
            {
                // only run notification logic if:
                // - The message is sent in a server text channel
                // - If the message mentions at least one user
                // - The message author is not a bot
                if (x.Channel is SocketTextChannel channel && x.MentionedUsers.Count > 0 && !x.Author.IsBot)
                {
                    // build the embed that gets sent to each mentioned user
                    EmbedBuilder eb = new EmbedBuilder()
                    {
                        Title = "You Were Mentioned",
                        Description = $"You were mentioned by {x.Author} in {channel.Guild.Name}\n__**Content:**__\n{x.Content}\n__**Responses:**__",
                        Color = Color.Orange
                    };
                    // add three inline fields to the embed
                    // TODO: make these not placeholders
                    eb.AddField("1", "Yes", true);
                    eb.AddField("2", "Maybe", true);
                    eb.AddField("3", "No", true);

                    // build the three buttons on the bottom of the message
                    // store the channel ID and message ID in the button's ID for later logic
                    // TODO: make these functional
                    ComponentBuilder cb = new ComponentBuilder();
                    cb.WithButton(customId: $"{channel.Id}_{x.Id}_one", style: ButtonStyle.Success, emote: oneEmoji);
                    cb.WithButton(customId: $"{channel.Id}_{x.Id}_two", style: ButtonStyle.Secondary, emote: twoEmoji);
                    cb.WithButton(customId: $"{channel.Id}_{x.Id}_three", style: ButtonStyle.Danger, emote: threeEmoji);

                    // loop through each mentioned user in the message
                    foreach (SocketUser user in x.MentionedUsers)
                    {
                        // if the mentioned user is not a human, ignore them
                        // TODO: ignore the user that sent the message as well, currently only allowed for testing purposes
                        if (user.IsBot || user.IsWebhook) continue;

                        // attempt to send a DM to the user containing the embed and buttons
                        try
                        {
                            IDMChannel dm = await user.CreateDMChannelAsync();
                            await dm.SendMessageAsync(embed: eb.Build(), components: cb.Build());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to send a DM to user {user}\n{e}");
                        }
                    }
                }
            };

            // lambda function for when a button is clicked
            client.ButtonExecuted += async (x) =>
            {
                // extract the data from the button's ID
                string[] data = x.Data.CustomId.Split('_');
                // get the channel of the original message
                SocketTextChannel? channel = await client.GetChannelAsync(ulong.Parse(data[0])) as SocketTextChannel;
                if (channel == null)
                {
                    Console.WriteLine("Failed to get channel on button click!");
                    return;
                }
                // get the original message
                SocketUserMessage? msg = await channel.GetMessageAsync(ulong.Parse(data[1])) as SocketUserMessage;
                if (channel == null)
                {
                    Console.WriteLine("Failed to get message on button click!");
                    return;
                }
                // handle the responses depending on the button clicked
                // this logic is temporary
                string response = data[2] switch
                {
                    "one" => "Yes",
                    "two" => "Maybe",
                    "three" => "No",
                    _ => throw new Exception("Unknown button ID value")
                };
                await confirmation(data, x.User, response);
                // build the response embed that gets sent back to the user
                EmbedBuilder eb = new EmbedBuilder()
                {
                    Title = "Quick Response",
                    Description = $"**{x.User}** responded with a quick response: {response}",
                    Color = Color.Orange
                };
                // reply to the original message with the response embed
                await msg.ReplyAsync(embed: eb.Build());
                // let the user know that the message got sent
                await x.RespondAsync($"Response sent successfully: {response}");
                // remove the buttons when we are done to prevent further interaction
                await x.Message.ModifyAsync(y => y.Components = new ComponentBuilder().Build());
            };

            // reflection stuff that the library uses
            interaction = new InteractionService(client);
            await interaction.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task Ready()
        {
            // once commands are registered with Discord, they do not have to be registered again
            // they only need to be re-registered if the name or parameters of the command change
            // therefore we can use a switch to only register the commands when we need to
            if (registerSlashCommands)
            {
                var commands = await interaction.RegisterCommandsGloballyAsync();
                Console.WriteLine($"Registered {commands.Count} slash commands with Discord.");
                registerSlashCommands = false;
            }
        }
        public async Task confirmation(string[] data, Discord.WebSocket.SocketUser user, string response)
        {
            EmbedBuilder eb = new EmbedBuilder()
            {
                Title = "Are You Sure",
                Description = $"you selected: {response}, are you sure about that?",
                Color = Color.Orange
            };
            eb.AddField("1", "Yes", true);
            eb.AddField("2", "No", true);

            ComponentBuilder cb = new ComponentBuilder();
            cb.WithButton(customId: $"one", style: ButtonStyle.Success, emote: oneEmoji);
            cb.WithButton(customId: $"two", style: ButtonStyle.Secondary, emote: twoEmoji);

            // attempt to send a DM to the user containing the embed and buttons
            try
            {
                IDMChannel dm = await user.CreateDMChannelAsync();
                await dm.SendMessageAsync(embed: eb.Build(), components: cb.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to send a DM to user {user}\n{e}");
            }
        }

        public async Task Start(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("You are missing a token.txt! The bot will not run without this!");
                Console.ReadLine();
                return;
            }

            // setting up the client
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                // this makes the bot always pre-download any users it can find for efficiency 
                AlwaysDownloadUsers = true,
                // the number of messages the bot will "remember" per channel
                // we don't need a very big cache size for our purposes
                MessageCacheSize = 10,
                // Discord permission stuff
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers
            });

            // Dependency Injection stuff which is utilized by the library
            // https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<DiscordSocketClient>()
                .BuildServiceProvider();

            // read the bot's token from the file
            // tokens are meant to be kept private, since anyone with the token can log in as the bot
            string token = File.ReadAllText("token.txt");

            // hook bot events
            await RegisterEvents();

            // here we set the switch for if we should register the commands
            // the only switch we check for is -r
            if (args.Length > 0 && args[0] == "-r")
            {
                registerSlashCommands = true;
            }

            // log in to Discord as the bot and start communicating with the API
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            // this just allows us to set the bot's status on Discord
            // uncomment if you want to
            // await client.SetGameAsync("for mentions!", type: ActivityType.Watching);

            // finally, we run the current Task indefinitely
            // basically this just means the bot will run forever until it crashes or is forced to shut down
            await Task.Delay(-1);
        }

    }
}
