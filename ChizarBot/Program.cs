using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace Chizar_Bot
{
    class Program
    {
        const string token = "";
        private List<SocketGuildUser> AllMembers = new List<SocketGuildUser>();
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;


        public async Task RunBotAsync()
        {
            Client = new DiscordSocketClient();
            Commands = new CommandService();
            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Client.Log += Client_Log;

            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, token);

            await Client.StartAsync();



            
     
            
            await Task.Delay(-1);

        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;
            Client.UserJoined += CheckBanList;
            Client.UserJoined += AnnaunceJoinedUser;
            Client.UserIsTyping += TypingMessage;

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

        private async Task TypingMessage(SocketUser arg1, ISocketMessageChannel arg2)
        {
            if (arg1.Id == 311865122815737856 || arg1.Id == 678596764881649665)
                await arg2.SendMessageAsync($"{arg1.Username} не пиши сюда, от тебя гавной воняет");
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg.Author.IsBot || arg.Author.IsWebhook || arg.Content.Length == 0) { Console.WriteLine("Returning..."); return; }
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(Client, message);

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await Commands.ExecuteAsync(context, argPos, Services);
                if (!result.IsSuccess) { Console.WriteLine(result.ErrorReason); }
            }
        }


        private async Task CheckBanList(SocketGuildUser user)
        {
            if (user.Id == 303918830072365056)
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync("Лохам Вход запрещён");
                await user.KickAsync("Artur Loh");
            }
        }

        public async Task AnnaunceJoinedUser(SocketGuildUser user)
        {
            var channel = Client.GetChannel(718185523390185577) as SocketTextChannel;
            await channel.SendMessageAsync($"Welcome {user.Mention} to {channel.Guild.Name}");
        }
    }
}
