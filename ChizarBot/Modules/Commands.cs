using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using ChizarBot;

namespace Chizar_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        
        [Command("init")]
        public async Task Loh()
        {
            IVoiceChannel channel = (IVoiceChannel)Context.Client.GetChannel(718514363459174540);
            IAudioClient client = await channel.ConnectAsync(false, false, true);     
        }

        [Command("ban")]
        public async Task VipisatBan()
        {
            if (!Program.IsAdmin(this.Context.User))
            {
                return;
            }
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            string[] args = message.ToString().Split(' ');
            ulong DiscId = Convert.ToUInt64(args[1]);
            ulong time = Convert.ToUInt64(args[2]);

            Program.BanList.Add(new BanMembers(DiscId, time));
        }

        [Command("CheckBanList")]
        public async Task PrintBanList()
        {
            foreach(BanMembers it in Program.BanList)
            {
                var message = this.Context.Message as SocketUserMessage;
                var context = new SocketCommandContext(this.Context.Client, message);
                await context.Channel.SendMessageAsync($"Лох {this.Context.Client.GetUser(it.DiscId1).Mention} забен на {it.Time}");

            }
        }

        [Command("unban")]
        public async Task UnBan()
        {
            if (!Program.IsAdmin(this.Context.User))
            {
                return;
            }
            var message = this.Context.Message as SocketUserMessage;
            var context = new SocketCommandContext(this.Context.Client, message);

            string[] args = message.ToString().Split(' ');
            ulong DiscId = Convert.ToUInt64(args[1]);
            ulong time = Convert.ToUInt64(args[2]);

            foreach(BanMembers it in Program.BanList)
            {
                if (it.DiscId1 == DiscId)
                {
                    Program.BanList.Remove(it);
                    break;
                }
            }
        }

    }
}
