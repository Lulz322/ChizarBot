using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Bot;

namespace Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private bool IsAdmin(SocketUser arg1, List<ulong> AdminList)
        {
            foreach (ulong it in AdminList)
            {
                if (arg1.Id == it)
                    return true;
            }
            return false;
        }

        [Command("ban")]
        public async Task VipisatBan(string id, string length)
        {
            List<ulong> Admins;
            Admins = ReadWriter.TakeUintList("./ini//AdminList");
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            if (!IsAdmin(message.Author, Admins)) { return; }
            string[] args = message.ToString().Split('|');
            ulong DiscId = Convert.ToUInt64(id);
            ulong time = Convert.ToUInt64(length);

            DateTime dataTime = System.DateTime.Now;
            dataTime = dataTime.AddMinutes(time);

            File.AppendAllText("./ini/BanList", id + "|" + context.Client.GetUser(DiscId).Username + "|" + dataTime + "\n");

            await context.Channel.SendMessageAsync($"Лошок  " +
                $"{this.Context.Client.GetUser(DiscId).Mention}" +
                $" был забанен"); 
        }
        [Command("ban")]
        public async Task VipisatBan(string id, string length, string comment)
        {
            List<ulong> Admins;
            Admins = ReadWriter.TakeUintList("./ini/AdminList");

            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            if (!IsAdmin(message.Author, Admins)) { return; }

            string[] args = message.ToString().Split(' ');
            ulong DiscId = Convert.ToUInt64(id);
            ulong time = Convert.ToUInt64(length);

            DateTime dataTime = System.DateTime.Now;
            dataTime = dataTime.AddMinutes(time);

            File.AppendAllText("./ini/BanList", id + "|" + context.Client.GetUser(DiscId).Username + "|" + dataTime + comment + "\n");

            await context.Channel.SendMessageAsync($"Лошок  " +
                $"{this.Context.Client.GetUser(DiscId).Mention}" +
                $" был забанен");
        }
        [Command("ban")]
        public async Task VipisatBan(string id, string length, string comment, string kickReason)
        {
            List<ulong> Admins;
            Admins = ReadWriter.TakeUintList("./ini/AdminList");
            
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);
            if (!IsAdmin(message.Author, Admins)) { return; }
            string[] args = message.ToString().Split('|');
            ulong DiscId = Convert.ToUInt64(id);
            ulong time = Convert.ToUInt64(length);

            DateTime dataTime = System.DateTime.Now;
            dataTime = dataTime.AddMinutes(time);

            File.AppendAllText("./ini/BanList", id + "|" + context.Client.GetUser(DiscId).Username + "|" + dataTime + "|" + comment + "|" + kickReason + "\n");

            await context.Channel.SendMessageAsync($"Лошок  " +
                $"{this.Context.Client.GetUser(DiscId).Mention}" +
                $" был забанен");
        }



        [Command("PrintBanList")]
        public async Task PrintBanList()
        {
            List<string> Bans;

            Bans = ReadWriter.TakeStringList("./ini/BanList");
            
            int i = 0;
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            foreach (string it in Bans)
            {
                string[] args = it.Split('|');
                string str = $"[{++i}] Лох {args[1]} с ID {args[0]} забен на {args[2]}";
                if (args.Length >= 4)
                    str += " | Comment: " + args[3];
                if (args.Length >= 5)
                    str += " | Kick reason: " + args[4];
                await context.Channel.SendMessageAsync(str);

            }
        }
        [Command("BanHelp")]
        public async Task BanHelp()
        {
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            await context.Channel.SendMessageAsync($"Как забанить лошка?\n" +
                $"!ban DiscID Length(In mitutes) @Comment @KickReason\n" +
                $"@Comment and @KickReason is not Neccessery");
        }
        [Command("unban")]
        public async Task UnBan(string uid)
        {
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            await context.Channel.SendMessageAsync($"Лошок с ID {uid} откинулся");
            await ReadWriter.RemoveObj(Convert.ToUInt64(uid), "./ini/BanList");
        }

    }
}
