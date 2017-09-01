using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : Singleton<Client>
{

    public NetworkClient client;
    public string ip = "127.0.0.1";
    public int port = 3000;



	void Awake () 
    {
        connect();

        registerHandler();
	}
	

    private void connect()
    {
        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        config.AddChannel(QosType.Unreliable);

        client = new NetworkClient();
        client.Configure(config, 1);
        client.Connect(ip, port);

        Log.Instance.Info("连接服务器......");
    }
    
    private void registerHandler()
    {
        client.RegisterHandler(MsgType.Connect, __onConn);
        client.RegisterHandler(MsgType.Disconnect, __onDisconn);

        client.RegisterHandler(MessageType.InfoNotify, __onInfoNotify);
        client.RegisterHandler(MessageType.Join, __onJoin);
        client.RegisterHandler(MessageType.Leave, __onLeave);
        client.RegisterHandler(MessageType.Move, __onMove);
        client.RegisterHandler(MessageType.GetOthers, __onGetOthers);
    }

    private void __onGetOthers(NetworkMessage netMsg)
    {
        Message.GetOthers notify = netMsg.ReadMessage<Message.GetOthers>();

        for(int i=0;i<notify.playerList.Count;i++)
        {
            GameManager.Instance.AddPlayer(notify.playerList[i]);
        }
    }

    private void __onInfoNotify(NetworkMessage netMsg)
    {
        Message.InfoNotify notify = netMsg.ReadMessage<Message.InfoNotify>();

        GameManager.Instance.myPlayerId = notify.playerId;
        Log.Instance.Info("服务器分配的playerId：" + notify.playerId);


        Message.Join join = new Message.Join();
        join.playerId = notify.playerId;
        client.Send(MessageType.Join, join);
    }

    private void __onLeave(NetworkMessage netMsg)
    {
        Message.Leave leave = netMsg.ReadMessage<Message.Leave>();

        GameManager.Instance.RemovePlayer(leave.playerId);
    }

    private void __onJoin(NetworkMessage netMsg)
    {
        Message.Join join = netMsg.ReadMessage<Message.Join>();

        Player p = new Player();
        p.playerId = join.playerId;
        p.pos = join.pos;
        p.direction = join.direction;

        GameManager.Instance.AddPlayer(p);
    }

    private void __onMove(NetworkMessage netMsg)
    {
        Message.Move m = netMsg.ReadMessage<Message.Move>();

        GameManager.Instance.RemovePlayer(m.playerId);
    }

    private void __onDisconn(NetworkMessage netMsg)
    {
        Log.Instance.Info("服务器断连");
    }

    private void __onConn(NetworkMessage netMsg)
    {
        Log.Instance.Info("已经连接到服务器");
    }
}
