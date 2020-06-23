using System;
using System.Collections.Generic;
using System.Text;

namespace ChizarBot
{
    class BanMembers
    {
        public BanMembers(ulong id, ulong bantime)
        {
            DiscId = id;
            time = bantime;
        }

        private ulong DiscId;
        private ulong time;

        public ulong DiscId1 { get => DiscId; set => DiscId = value; }
        public ulong Time { get => time; set => time = value; }
    }
}
