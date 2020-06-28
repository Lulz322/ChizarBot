using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        public List<ulong> AdminList;
        public List<BanMembers> BanList = new List<BanMembers>();
        private List<string> Answers;
        private List<ulong> TypingList;
        private List<ulong> ReactToMessage;
        bool FirstRun = true;
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;

        private string nowtime;

        //private FileSystemWatcher Watcher = new FileSystemWatcher();



        private async void ChangeTime()
        {
            IEnumerator<SocketGuildUser> it;
            while (true)
            {
                nowtime = System.DateTime.Now.ToString();
                Thread.Sleep(1000);
                await CheckBanListTwo();
                it = Client.GetGuild(718185523390185574).Users.GetEnumerator();
                while (it.MoveNext())
                {
                    if (!it.Current.IsBot)
                        await NeedSomeoneToBan(it.Current);
                }
            }
        }

        public async Task RunBotAsync()
        {
            Client = new DiscordSocketClient();
            Commands = new CommandService();
            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Client.Log += Client_Log;

            TakeBanList();

            Answers = ReadWriter.TakeStringList("Answers");
            TypingList = ReadWriter.TakeUintList("TypingList");
            AdminList = ReadWriter.TakeUintList("AdminList");

/*            Watcher.Path = @"Y:\Projects\Chizar\ChizarBot\bin\Debug\netcoreapp3.1";
            Watcher.NotifyFilter = NotifyFilters.LastWrite;
            Watcher.Filter = "*.*";
*/




            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, token);

            await Client.StartAsync();
            Thread myThread = new Thread(new ThreadStart(ChangeTime));
            myThread.Start();

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
            //Watcher.Changed += onBanChange;
            Client.Ready += CheckConnectedVariables;


            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

        private async Task CheckConnectedVariables()
        {
            IVoiceChannel channel = (IVoiceChannel)Client.GetChannel(718514363459174540);
            IAudioClient client = await channel.ConnectAsync(false, false, true);
        }

        private void onBanChange(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed && e.Name == "BanList")
            {
                BanList.Clear();
                List<string> tmp = ReadWriter.TakeStringList("BanList");
                foreach (string it in tmp)
                {
                    string[] strtmp = it.Split('|');
                    BanList.Add(new BanMembers(Convert.ToUInt64(strtmp[0]),
                        strtmp[1], strtmp[2], strtmp[3], strtmp[4]));
                }
            }

        }

        private void TakeBanList()
        {
            BanList.Clear();
            List<string> tmp = ReadWriter.TakeStringList("BanList");
            foreach (string it in tmp)
            {
                string[] strtmp = it.Split('|');
                if (strtmp.Length == 3)
                    BanList.Add(new BanMembers(Convert.ToUInt64(strtmp[0]),
                        strtmp[1], strtmp[2], null, null));
                else if (strtmp.Length == 4)
                    BanList.Add(new BanMembers(Convert.ToUInt64(strtmp[0]),
                        strtmp[1], strtmp[2], strtmp[3], null));
                else if (strtmp.Length == 5)
                    BanList.Add(new BanMembers(Convert.ToUInt64(strtmp[0]),
                        strtmp[1], strtmp[2], strtmp[3], strtmp[4]));
            }
        }

        private async Task CheckBanListTwo()
        {
            TakeBanList();
            foreach (BanMembers it in BanList)
            {
                if (it.GetTime() == nowtime.ToString())
                {
                    var context = Client.GetChannel(718185523390185577) as SocketTextChannel;
                    await context.SendMessageAsync($"Лошок {it.GetName()} с ID {it.GetDiscId()} откинулся");

                    await ReadWriter.RemoveObj(it.GetDiscId(), "BanList");
                }
            }
        }



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

        public bool IsAdmin(SocketUser arg1)
        {
            foreach(ulong it in AdminList)
            {
                if (arg1.Id == it)
                    return true;
            }
            return false;
        }

        private async Task TypingMessage(SocketUser arg1, ISocketMessageChannel arg2)
        {
            foreach(ulong it in TypingList)
            {
                if (it == arg1.Id)
                    await arg2.SendMessageAsync($"{arg1.Mention}, не пиши сюда, от тебя гавной воняет");
            }

        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg.Author.IsBot || arg.Author.IsWebhook || arg.Content.Length == 0) { Console.WriteLine("Returning..."); return; }
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(Client, message);


            if (context.Channel.Id == 718888382176165968)
            {
                await context.Channel.SendMessageAsync($"{arg.Author.Username} начинает играть в " + message.ToString() + ". Присойденяйся, Сань");
                await message.DeleteAsync();
                return;
            }

            if (context.Channel.Id == 726764864164331521)
            {
                await context.Channel.SendMessageAsync($"{arg.Author.Username} Ролит на " + message.ToString());
                await message.DeleteAsync();
                return;
            }


            if (arg.Author.Id == 219818135748870144)
            {
                await context.Channel.SendMessageAsync("Спасибо, Дэнчик!");
            }
            else
            {
                if (!IsAdmin(arg.Author))
                {
                    Random random = new Random();

                    string text = Answers.ElementAt(random.Next(0, Answers.Count));
                    var channel = new SocketCommandContext(Client, message);
                    await arg.Channel.SendMessageAsync($"{arg.Author.Mention}, {text}");
                }
            }


            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await Commands.ExecuteAsync(context, argPos, Services);
                if (!result.IsSuccess) { Console.WriteLine(result.ErrorReason); }
            }
        }

        private async Task NeedSomeoneToBan(SocketGuildUser user)
        {
            foreach (BanMembers it in BanList)
            {
                if (it.GetDiscId() == user.Id)
                {
                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    await dmChannel.SendMessageAsync("Лохам Вход запрещён");
                    await user.KickAsync($"{it.GetKickWithReason()} Лошок");
                    return;
                }
            }
        }

        private async Task CheckBanList(SocketGuildUser user)
        {
            foreach (BanMembers it in BanList)
            {
                if (it.GetDiscId() == user.Id)
                {
                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    var channel = Client.GetChannel(718185523390185577) as SocketTextChannel;
                    await channel.SendMessageAsync($"Лох { user.Mention } ID: { user.Id}\nХотел зайти на канал");
                    await dmChannel.SendMessageAsync("Лохам Вход запрещён");
                    await user.KickAsync($"{it.GetKickWithReason()} Лошок");
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
