using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Message
{
    public class Join : MessageBase
    {
        public uint playerId;
        public Vector3 pos;
        public Vector3 direction;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(playerId);
            writer.Write(pos);
            writer.Write(direction);
        }

        public override void Deserialize(NetworkReader reader)
        {
            playerId = reader.ReadUInt32();
            pos = reader.ReadVector3();
            direction = reader.ReadVector3();
        }
    }

    public class Leave : MessageBase
    {
        public uint playerId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(playerId);
        }

        public override void Deserialize(NetworkReader reader)
        {
            playerId = reader.ReadUInt32();
        }
    }

    public class InfoNotify : MessageBase
    {
        public uint playerId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(playerId);
        }
    }


    public class Move : MessageBase
    {
        public int gameFrame;
        public uint playerId;
        public Vector3 direction;
        public float speed;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(gameFrame);
            writer.Write(playerId);
            writer.Write(direction);
            writer.Write(speed);
        }

        public override void Deserialize(NetworkReader reader)
        {
            gameFrame = reader.ReadInt32();
            playerId = reader.ReadUInt32();
            direction = reader.ReadVector3();
            speed = reader.ReadSingle();
        }
    }

    public class GetOthers : MessageBase
    {
        public List<Player> playerList = new List<Player>();

        public override void Deserialize(NetworkReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Player p = new Player();
                p.playerId = reader.ReadUInt32();
                p.pos = reader.ReadVector3();
                p.direction = reader.ReadVector3();

                playerList.Add(p);
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(playerList.Count);
            for(int i=0;i<playerList.Count;i++)
            {
                writer.Write(playerList[i].playerId);
                writer.Write(playerList[i].pos);
                writer.Write(playerList[i].direction);
            }
        }
    }

}
