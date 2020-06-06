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
        
        [Command("init")]
        public async Task Loh()
        {
            IVoiceChannel channel = (IVoiceChannel)Context.Client.GetChannel(718514363459174540);
            IAudioClient client = await channel.ConnectAsync(false, false, true);     
        }
    }
}
