using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bot;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace Bot
{
    class Program
    {
        const string token = "";
        private List<SocketGuildUser> AllMembers = new List<SocketGuildUser>();
        public List<ulong> AdminList;
        public List<BanMembers> BanList = new List<BanMembers>();
        private List<string> Answers;
        private List<MemberMovements> ConnectedUsers;
        private List<ulong> TypingList;
        //private List<ulong> ReactToMessage;
        bool FirstRun = true;
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;
        private static bool isNeedToCheck = false;
        private string nowtime;
        private DateTime nowatime;

        private bool HourTick = false;

        //private FileSystemWatcher Watcher = new FileSystemWatcher();



        private async void ChangeTime()
        {
            IEnumerator<SocketGuildUser> it;
            while (true)
            {
                nowtime = System.DateTime.Now.ToString();
                nowatime = System.DateTime.Now;
		        nowatime = nowatime.AddHours(3);
                Thread.Sleep(1000);
                await CheckBanListTwo();
                if (isNeedToCheck == true)
                {
                    it = Client.GetGuild(718185523390185574).Users.GetEnumerator();
                    while (it.MoveNext())
                    {
                        if (!it.Current.IsBot || !IsAdmin(it.Current))
                            await NeedSomeoneToBan(it.Current);
                    }
                    isNeedToCheck = false;
                }
            }
        }


        private async void EveryHourBan(){
            DateTime TimerToBan = nowatime;
            IReadOnlyCollection<SocketGuild> temp;
            IEnumerator<SocketGuildUser> it;
            List<SocketUser> UsersOnChannel = new List<SocketUser>();
            var dm = Client.GetChannel(718185523390185577) as SocketTextChannel;


            while (true){
                if (!HourTick){
                    TimerToBan = nowatime.AddHours(1);
                    HourTick = true;
                }
                if (nowatime == TimerToBan){
                    temp = Client.Guilds;
                    foreach(SocketGuild iterator in temp){
                        it = iterator.Users.GetEnumerator();
                        while(it.MoveNext()){
                            if (!it.Current.IsBot || !IsAdmin(it.Current))
                                UsersOnChannel.Add(it.Current);
                        }
                        Random random = new Random();
                        
                        BanMember(UsersOnChannel[random.Next(0, UsersOnChannel.Count)], nowatime.AddHours(1), "Для профилактики", "Профилактика, лошок");
                        HourTick = false;
                    }
                }
                if (TimerToBan.AddMinutes(-5) == nowatime){
                    await dm.SendMessageAsync("До профилактического бана осталось 5 минут");
                }else if(TimerToBan.AddMinutes(-3) == nowatime){
                    await dm.SendMessageAsync("До профилактического бана осталось 3 минуты");
                }else if (TimerToBan.AddMinutes(-1) == nowatime){
                    await dm.SendMessageAsync("До профилактического бана осталось 1 минута");
                }else if(TimerToBan.AddMinutes(-15) == nowatime){
                    await dm.SendMessageAsync("До профилактического бана осталось 15 минут");
                }else if (TimerToBan.AddMinutes(-30) == nowatime){
                    await dm.SendMessageAsync("До профилактического бана осталось 30 минут");
                }else if (TimerToBan.AddMinutes(-45) == nowatime){
                    await dm.SendMessageAsync("До профилактического бана осталось 45 минут");
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

            Answers = ReadWriter.TakeStringList("./ini/Answers");
            TypingList = ReadWriter.TakeUintList("./ini/TypingList");
            AdminList = ReadWriter.TakeUintList("./ini/AdminList");
            ConnectedUsers = new List<MemberMovements>();

/*            Watcher.Path = @"Y:\Projects\Chizar\ChizarBot\bin\Debug\netcoreapp3.1";
            Watcher.NotifyFilter = NotifyFilters.LastWrite;
            Watcher.Filter = "*.*";
*/




            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, token);

                await Client.StartAsync();


            try
            {
                Thread myThread = new Thread(new ThreadStart(ChangeTime));
                Thread HourThread = new Thread(new ThreadStart(EveryHourBan));
                myThread.Start();
                HourThread.Start();
            }catch(Exception e)
            {
                Console.WriteLine("THREAD");
            }


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
            Client.UserVoiceStateUpdated += VoiceUpdated;
            
            
            //Watcher.Changed += onBanChange;
            Client.Ready += CheckConnectedVariables;


            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }


        MemberMovements IsMemberExist(ulong id){
            foreach(MemberMovements it in ConnectedUsers){
                if (id == it.GetId()){
                    return it;
                }
            }
            return null;
        }

        private async Task VoiceUpdated(SocketUser user , SocketVoiceState firstState, SocketVoiceState secondState){
            var dm = Client.GetChannel(737666532171317329) as SocketTextChannel;
            if (IsAdmin(user))
                return ;
            if (firstState.VoiceChannel == null){
                await dm.SendMessageAsync($"{user.Mention} connected to {secondState.VoiceChannel.Name} {nowatime.ToString()}");
                MemberMovements temp = IsMemberExist(user.Id);
                if (temp == null)
                    ConnectedUsers.Add(new MemberMovements(user.Id, nowatime));
                else{
                    if (nowatime > temp.GetTime().AddHours(1)){
                        ConnectedUsers.Remove(temp);
                        return ;
                    }
                    temp.increment();
                    if (temp.GetConnectedTime() == 2){
                        await user.SendMessageAsync($"If you connected/disconnected one more time in short time on {dm.Guild.Name} u'll get banned");
                        await dm.SendMessageAsync($"{user.Mention} already connected two times in one hour, next one will get banned");
                    }
                    if (temp.GetConnectedTime() == 3){
                        BanMember(user, nowatime.AddHours(1), "Connected a lot of times in short period", "A lot of connetcions to server");
                        ConnectedUsers.Remove(temp);
                    }
                }
                return ;
            }
            if (secondState.VoiceChannel == null){
                await dm.SendMessageAsync($"{user.Mention} disconneted from {firstState.VoiceChannel.Name} {nowatime.ToString()}");
                return ;
            }

            if (secondState.IsSelfMuted && secondState.IsSelfDeafened){
                await dm.SendMessageAsync($"{user.Mention} Muted and Deafened {nowatime.ToString()}");
            }else if (secondState.IsSelfMuted){
                await dm.SendMessageAsync($"{user.Mention} Muted {nowatime.ToString()}");
            }else if (secondState.IsSelfDeafened){
                await dm.SendMessageAsync($"{user.Mention} Deafened {nowatime.ToString()}");
            }else if (secondState.VoiceChannel != firstState.VoiceChannel){
                await dm.SendMessageAsync($"{user.Mention} moved from {firstState.VoiceChannel.Name} to {secondState.VoiceChannel.Name} {nowatime.ToString()}");
            }

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
                List<string> tmp = ReadWriter.TakeStringList("./ini/BanList");
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
            List<string> tmp = ReadWriter.TakeStringList("./ini/BanList");
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
            Program.isNeedToCheck = true;
        }

        private async Task CheckBanListTwo()
        {
            TakeBanList();
            foreach (BanMembers it in BanList)
            {
                if (it.GetTime() == nowtime.ToString())
                {
                    try
                    {
                        var context = Client.GetChannel(718185523390185577) as SocketTextChannel;
                        await context.SendMessageAsync($"Лошок {it.GetName()} с ID {it.GetDiscId()} откинулся");

                        await ReadWriter.RemoveObj(it.GetDiscId(), "BanList");
                        var dmChannel = await Client.GetUser(it.GetDiscId()).GetOrCreateDMChannelAsync();
                        await dmChannel.SendMessageAsync($"Ваши Грехи были опущенны на сервере {context.Guild.Name}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("CheckBanListTwo Exception");
                    }
                    
                }
            }
        }

        private async void BanMember(SocketUser user, DateTime date, string cmt, string kick_reason){
            var dm = Client.GetChannel(718185523390185577) as SocketTextChannel;
            
            await dm.SendMessageAsync($"Лох {user.Mention} был забанен");
            File.AppendAllText("./ini/BanList", user.Id + "|" + user.Username + "|" + date + "|" + cmt + "|" + kick_reason + "\n");
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
                    try
                    {
                        var dmChannel = await user.GetOrCreateDMChannelAsync();
                        await dmChannel.SendMessageAsync($"Лоху Вход запрещён до {it.GetTime()}");
                        await user.KickAsync($"{it.GetKickWithReason()} Лошок");
                    }catch(Exception e)
                    {
                        Console.WriteLine("Need sometoBan Exception");
                    }

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
                    try
                    {
                        var dmChannel = await user.GetOrCreateDMChannelAsync();
                        var channel = Client.GetChannel(718185523390185577) as SocketTextChannel;
                        await channel.SendMessageAsync($"Лох { user.Mention } ID: { user.Id}\nХотел зайти на канал");
                        await dmChannel.SendMessageAsync($"Лоху Вход запрещён до {it.GetTime()}");
                        await user.KickAsync($"{it.GetKickWithReason()} Лошок");
                        return;
                    }
                    catch
                    {
                        Console.WriteLine("CheckBanList Exception");
                    }

                }
            }
        }

        public async Task AnnaunceJoinedUser(SocketGuildUser user)
        {
            var channel = Client.GetChannel(718185523390185577) as SocketTextChannel;
            await channel.SendMessageAsync($"Приветсвую, {user.Mention}.\nНа сервере работает санитар {Client.CurrentUser.Mention}");
        }
    }
}
