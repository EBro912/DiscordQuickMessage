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
 *  OpenAI_API
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

        // a dictionary to store current active quick message responses
        // the key is the user id and the value is the corresponding information to be used later
        private Dictionary<ulong, QuickMessage> activeQuickMessages = new Dictionary<ulong, QuickMessage>();

        // a dictionary to store current confirmations
        // the key is the user id and the value is the response the user is confirming
        private Dictionary<ulong, string> activeConfirmations = new Dictionary<ulong, string>();

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
                // - The message content is greater than 0
                if (x.Channel is SocketTextChannel channel && x.MentionedUsers.Count > 0 && !x.Author.IsBot && x.Content.Length > 0)
                {
                    _ = Task.Run(() => HandleSendResponses(x));
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
                if (!activeQuickMessages.TryGetValue(x.User.Id, out QuickMessage quickMessage))
                {
                    Console.WriteLine("Failed to get QuickMessage from active quick messages for User ID: " + x.Message.Id);
                    return;
                }

                // if the button is from a confirmation, use different logic
                if (data[2] == "confirm")
                {
                    // if the user declines the confirmation, rebuild the original embed
                    if (data[3] == "no")
                    {
                        EmbedBuilder en = quickMessage.Embed;
                        // build the four buttons on the bottom of the message
                        // store the channel ID and message ID in the button's ID for later logic
                        ComponentBuilder cn = new ComponentBuilder();
                        cn.WithButton(customId: $"{data[0]}_{data[1]}_one", style: ButtonStyle.Success, emote: oneEmoji);
                        cn.WithButton(customId: $"{data[0]}_{data[1]}_two", style: ButtonStyle.Secondary, emote: twoEmoji);
                        cn.WithButton(customId: $"{data[0]}_{data[1]}_three", style: ButtonStyle.Danger, emote: threeEmoji);
                        cn.WithButton(customId: $"{data[0]}_{data[1]}_ignore", style: ButtonStyle.Danger, label: "Ignore");
                        cn.WithButton(style: ButtonStyle.Link, label: "Original Message", url: quickMessage.JumpUrl);
                        await x.Message.ModifyAsync(x =>
                        {
                            x.Embed = en.Build();
                            x.Components = cn.Build();
                        });
                        activeConfirmations.Remove(x.User.Id);
                        await x.DeferAsync();
                        return;
                    }

                    if (!activeConfirmations.TryGetValue(x.User.Id, out string result))
                    {
                        Console.WriteLine("Failed to get response from active confirmation for User ID: " + x.Message.Id);
                        return;
                    }
                    // build the response embed that gets sent back to the user
                    EmbedBuilder eb = new EmbedBuilder()
                    {
                        Title = "Quick Response",
                        Description = $"**{x.User}** responded with a quick response: {result}",
                        Color = Color.Orange
                    };
                    // reply to the original message with the response embed
                    await msg.ReplyAsync(embed: eb.Build());
                    // remove the quick response from the active responses
                    activeQuickMessages.Remove(x.Message.Id);
                    activeConfirmations.Remove(x.User.Id);
                    // let the user know that the message got sent
                    await x.RespondAsync($"Response sent successfully: {result}");
                    // delete the message to prevent further interaction
                    await x.Message.DeleteAsync();
                    return;
                }

                // if the user ignores the ping, simply remove message and do nothing
                if (data[2] == "ignore")
                {
                    await x.Message.DeleteAsync();
                    activeQuickMessages.Remove(x.User.Id);
                    return;
                }

                string response = data[2] switch
                {
                    "one" => quickMessage.PositiveResponse,
                    "two" => quickMessage.NeutralResponse,
                    "three" => quickMessage.NegativeResponse,
                    _ => throw new ArgumentException("Unknown switch input: " + data[2])
                };

                // build confirmation embed
                EmbedBuilder ec = new EmbedBuilder()
                {
                    Title = "Response Confirmation:",
                    Description = $"Are you sure you want to send the following response?\n{response}",
                    Color = Color.Red
                };

                // transmit the same data over to the confirmation buttons
                ComponentBuilder cb = new ComponentBuilder();
                cb.WithButton(customId: $"{data[0]}_{data[1]}_confirm_yes", style: ButtonStyle.Success, label: "Yes");
                cb.WithButton(customId: $"{data[0]}_{data[1]}_confirm_no", style: ButtonStyle.Danger, label: "No");

                await x.Message.ModifyAsync(x =>
                {
                    x.Embed = ec.Build();
                    x.Components = cb.Build();
                });
                activeConfirmations.Add(x.User.Id, response);
                await x.DeferAsync();
            };

            // reflection stuff that the library uses
            interaction = new InteractionService(client);
            await interaction.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleSendResponses(SocketMessage x)
        {
            SocketTextChannel channel = (SocketTextChannel)x.Channel;
            // build the embed that gets sent to each mentioned user
            EmbedBuilder eb = new EmbedBuilder()
            {
                Title = "You Were Mentioned",
                Description = $"You were mentioned by {x.Author} in {channel.Guild.Name}\n__**Content:**__\n{x.Content}\n__**Responses:**__",
                Color = Color.Orange
            };

            string jumpUrl = x.GetJumpUrl();

            // TODO: possibly ignore certain "invalid" messages
            // i.e. messages that are too long, arent a question, etc.
            QuickMessage quickMessage = new QuickMessage(x.CleanContent, eb, jumpUrl);
            await quickMessage.GenerateResponses();
            // add three inline fields to the embed
            eb.AddField("1", quickMessage.PositiveResponse, true);
            eb.AddField("2", quickMessage.NeutralResponse, true);
            eb.AddField("3", quickMessage.NegativeResponse, true);

            // build the four buttons on the bottom of the message
            // store the channel ID and message ID in the button's ID for later logic
            ComponentBuilder cb = new ComponentBuilder();
            cb.WithButton(customId: $"{channel.Id}_{x.Id}_one", style: ButtonStyle.Success, emote: oneEmoji);
            cb.WithButton(customId: $"{channel.Id}_{x.Id}_two", style: ButtonStyle.Secondary, emote: twoEmoji);
            cb.WithButton(customId: $"{channel.Id}_{x.Id}_three", style: ButtonStyle.Danger, emote: threeEmoji);
            cb.WithButton(customId: $"{channel.Id}_{x.Id}_ignore", style: ButtonStyle.Danger, label: "Ignore");
            cb.WithButton(style: ButtonStyle.Link, label: "Original Message", url: jumpUrl);

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
                    // currently, all of the users mentioned will share the same three prompts
                    // maybe TODO: change this so they are each unique?
                    activeQuickMessages.Add(user.Id, quickMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to send a DM to user {user}\n{e}");
                }
            }
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

        public async Task Start(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("You are missing a token.txt! The bot will not run without this!");
                Console.ReadLine();
                return;
            }

            // initialize the OpenAIHandler
            OpenAIHandler.Init();

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