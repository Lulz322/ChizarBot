using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChizarBot;
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
        private List<ulong> TypingList;
        private List<ulong> BanList;
        private List<ulong> ReactToMessage;
        bool FirstRun = true;
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

            BanList = ReadWriter.TakeList("BanList");
            TypingList = ReadWriter.TakeList("TypingList");


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
            Client.GuildAvailable += Tick;
            //Client.Connected += CheckConnectedVariables;
            
            

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

/*        private async Task CheckConnectedVariables()
        {

        }*/

        private async Task Tick(SocketGuild arg)
        {
            if (FirstRun)
            {
                IEnumerator<SocketGuildUser> it;

                it = arg.Users.GetEnumerator();
                while (it.MoveNext())
                {
                    if (!it.Current.IsBot)
                        await CheckBanList(it.Current);
                }
                FirstRun = false;
            }
        }


        private async Task TypingMessage(SocketUser arg1, ISocketMessageChannel arg2)
        {
            foreach (ulong it in TypingList)
            {
                if (it == arg1.Id)
                {
                    await arg2.SendMessageAsync($"{arg1.Mention} не пиши сюда, от тебя гавной воняет");
                    return;
                }
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg.Author.IsBot || arg.Author.IsWebhook || arg.Content.Length == 0) { Console.WriteLine("Returning..."); return; }
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(Client, message);


            if (context.Channel.Id == 718888382176165968)
            {
                await context.Channel.SendMessageAsync($"{arg.Author.Username} начинает играть в" + message.ToString() + ". Присойденяйся, Сань");
                await message.DeleteAsync();
                return;
            }
            

            if (arg.Author.Id == 219818135748870144)
                await context.Channel.SendMessageAsync("Спасибо Дэнчик!");


            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await Commands.ExecuteAsync(context, argPos, Services);
                if (!result.IsSuccess) { Console.WriteLine(result.ErrorReason); }
            }
        }


        private async Task CheckBanList(SocketGuildUser user)
        {
            foreach (ulong it in BanList)
            {
                if (it == user.Id)
                {
                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    var channel = Client.GetChannel(718185523390185577) as SocketTextChannel;
                    await channel.SendMessageAsync($"Лох { user.Nickname}ID: { user.Id}\nХотел зайти на канал");
                    await dmChannel.SendMessageAsync("Лохам Вход запрещён");
                    await user.KickAsync($"{user.Mention} Лошок");
                    return;
                }
            }
        }

        public async Task AnnaunceJoinedUser(SocketGuildUser user)
        {
            var channel = Client.GetChannel(718185523390185577) as SocketTextChannel;
            await channel.SendMessageAsync($"Welcome {user.Mention} to {channel.Guild.Name}");
        }
    }
}
