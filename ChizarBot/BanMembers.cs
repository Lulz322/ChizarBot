using System;
using System.Collections.Generic;
using System.Text;

namespace ChizarBot
{
    class BanMembers
    {
        public BanMembers(ulong id, string nm, string date, string cm, string kick)
        {
            Name = nm;
            DiscId = id;
            time = date;
            comment = cm;
            KickWithReason = kick;
        }

        private ulong DiscId;
        private string time;
        private string Name;
        private string comment;
        private string KickWithReason;


        public string GetComment() { return comment; }
        public string GetKickWithReason() { return KickWithReason; }

        public string GetName() { return Name; }
        public ulong GetDiscId() { return DiscId; }
        public string GetTime() { return time; }

    }
}
