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

            // TODO: register event to handle when a user is mentioned

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
                GatewayIntents = GatewayIntents.AllUnprivileged
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