using Bot;
using System;

class MemberMovements{
    public MemberMovements(ulong DiscordId, DateTime FirstConnetionTime){
        id = DiscordId;
        time = FirstConnetionTime;
        connected_time = 1;
    }


    public ulong GetId(){
        return id;
    }
    
    public int GetConnectedTime(){
        return connected_time;
    }
    public void increment(){
        connected_time++;
    }

    public DateTime GetTime(){
        return time;
    }

    private ulong id;
    private int connected_time;
    private DateTime time;
};