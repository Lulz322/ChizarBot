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

namespace Chizar_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private IEnumerator<SocketGuildUser> it;

        [Command("aue")]
        public async Task Ping()
        {
            it =  this.Context.Guild.Users.GetEnumerator();
            while (it.MoveNext())
            {
                if (!it.Current.IsBot)
                {
                    var dmChannel = await it.Current.GetOrCreateDMChannelAsync();
                    await dmChannel.SendMessageAsync("Фарту Масти Ауе https://www.meme-arsenal.com/memes/654509799044878ae2aa6a7b1da11d5c.jpg");
                    
                }
            }
            await ReplyAsync("- АУЕ");
        }
        [Command("loh")]
        public async Task Loh()
        {
            IVoiceChannel channel = (IVoiceChannel)Context.Client.GetChannel(718514363459174540);
            IAudioClient client = await channel.ConnectAsync(false, false, true);
            

            
        }
    }
}
