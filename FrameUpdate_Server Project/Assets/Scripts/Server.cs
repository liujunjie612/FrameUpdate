using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : Singleton<Server> 
{
    public string ip = "127.0.0.1";
    public int port = 3000;
    public int maxConnenction = 100;

    private uint Id = 0;

    private int GameFrame = 0;

    private float AccumilatedTime = 0f;

    private float FrameLength = 0.05f; //50 miliseconds

    public Dictionary<int, Player> playerDic = new Dictionary<int, Player>();  

    void Start()
    {
        startServer();
    }
	
    void Update()
    {
		AccumilatedTime = AccumilatedTime + Time.deltaTime;
		
		while(AccumilatedTime > FrameLength) {
            GameFrame++;
            Log.Instance.Info(GameFrame);
			AccumilatedTime = AccumilatedTime - FrameLength;
		}
    }

    private void startServer()
    {
        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        config.AddChannel(QosType.Unreliable);
        HostTopology host = new HostTopology(config, maxConnenction);
        NetworkServer.Configure(host);
        if (NetworkServer.Listen(port))
        {
            NetworkServer.RegisterHandler(MsgType.Connect, __onConn);
            NetworkServer.RegisterHandler(MsgType.Disconnect, __onDisconn);

            NetworkServer.RegisterHandler(MessageType.Join, __onJoin);
            NetworkServer.RegisterHandler(MessageType.Move, __onMove);
        }
        Log.Instance.Info("服务器已开启");
    }

    private void __onJoin(NetworkMessage netMsg)
    {
        Message.Join join = netMsg.ReadMessage<Message.Join>();

        float f = Random.Range(0, 10);
        join.pos = new Vector3(f, f, f);
        join.direction = new Vector3(f, f, f);
        NetworkServer.SendToAll(MessageType.Join, join);


        Message.GetOthers n = new Message.GetOthers();
        Player[] array = new Player[playerDic.Count];
        playerDic.Values.CopyTo(array, 0);
        n.playerList = new List<Player>(array);
        NetworkServer.SendToClient(netMsg.conn.connectionId, MessageType.GetOthers, n);


        if (!playerDic.ContainsKey(netMsg.conn.connectionId))
        {
            Player p = new Player();
            p.playerId = join.playerId;
            p.pos = join.pos;
            p.direction = join.direction;

            playerDic.Add(netMsg.conn.connectionId, p);
        }
    }

    private void __onMove(NetworkMessage netMsg)
    {
        Message.Move m = netMsg.ReadMessage<Message.Move>();
        NetworkServer.SendToAll(MessageType.Move, m);
    }

    private void __onDisconn(NetworkMessage netMsg)
    {
        if (playerDic.ContainsKey(netMsg.conn.connectionId))
        {
            Message.Leave leave = new Message.Leave();
            leave.playerId = playerDic[netMsg.conn.connectionId].playerId;

            NetworkServer.SendToAll(MessageType.Leave, leave);
            

            playerDic.Remove(netMsg.conn.connectionId);
        }
        Log.Instance.Info(string.Format("Client: {0} 下线", netMsg.conn.connectionId));
    }

    private void __onConn(NetworkMessage netMsg)
    {
        NetworkServer.SetClientReady(netMsg.conn);

        Message.InfoNotify notify = new Message.InfoNotify();
        Id++;
        notify.playerId = Id;
        NetworkServer.SendToClient(netMsg.conn.connectionId, MessageType.InfoNotify, notify);

        Log.Instance.Info(string.Format("Client: {0} 连接到服务器, 分配给的playerId：{1}", netMsg.conn.connectionId, Id));
    }
}
