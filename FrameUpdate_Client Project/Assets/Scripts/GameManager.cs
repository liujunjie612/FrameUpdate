using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager :Singleton<GameManager> 
{
    public GameObject obj;
    public uint myPlayerId;
    public GameObject myPlayerObj;

    public Dictionary<uint, Player> playerInfoDic = new Dictionary<uint, Player>();
    public Dictionary<uint, GameObject> playerObjDic = new Dictionary<uint, GameObject>();
    void Start()
    {
        Client.Instance.client.RegisterHandler(MessageType.Move, __onMove);
    }

    private void __onMove(UnityEngine.Networking.NetworkMessage netMsg)
    {
        Message.Move move = netMsg.ReadMessage<Message.Move>();

        GetPlayerObj(move.playerId).transform.Translate(move.direction * move.speed);
    }

    private float AccumilatedTime = 0f;

    private float FrameLength = 0.05f; //50 miliseconds

    private float moveSpeed = 0.01f;

    void Update()
    {
        //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength
        AccumilatedTime = AccumilatedTime + Time.deltaTime;

        //in case the FPS is too slow, we may need to update the game multiple times a frame
        while (AccumilatedTime > FrameLength)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Message.Move move = new Message.Move();
                move.direction = Vector3.forward;
                move.playerId = myPlayerId;
                move.speed = moveSpeed;
                Client.Instance.client.Send(MessageType.Move, move);
                //myPlayerObj.transform.Translate(Vector3.forward * 0.5f);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Message.Move move = new Message.Move();
                move.direction = Vector3.back;
                move.playerId = myPlayerId;
                move.speed = moveSpeed;
                Client.Instance.client.Send(MessageType.Move, move);
                //myPlayerObj.transform.Translate(Vector3.back * 0.5f);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Message.Move move = new Message.Move();
                move.direction = Vector3.left;
                move.playerId = myPlayerId;
                move.speed = moveSpeed;
                Client.Instance.client.Send(MessageType.Move, move);
                //myPlayerObj.transform.Translate(Vector3.left * 0.5f);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Message.Move move = new Message.Move();
                move.direction = Vector3.right;
                move.playerId = myPlayerId;
                move.speed = moveSpeed;
                Client.Instance.client.Send(MessageType.Move, move);
                //myPlayerObj.transform.Translate(Vector3.right * 0.5f);
            }


            AccumilatedTime = AccumilatedTime - FrameLength;
        }

       
    }

    //private int GameFrame = 0;
    //private void GameFrameTurn()
    //{
    //    if (GameFrame == 0)
    //    {
    //        if (LockStepTurn())
    //        {
    //            GameFrame++;
    //        }
    //    }
    //    else
    //    {
    //        GameFrame++;
    //        if (GameFrame == GameFramesPerLocksetpTurn)
    //        {
    //            GameFrame = 0;
    //        }
    //    }
    //}

    public void AddPlayer(Player p)
    {
        if (!playerInfoDic.ContainsKey(p.playerId))
        {
            playerInfoDic.Add(p.playerId, p);

            GameObject pObj = Instantiate(obj);
            pObj.transform.position = p.pos;
            pObj.transform.localEulerAngles = p.direction;

            if (p.playerId == myPlayerId)
                myPlayerObj = pObj;

            playerObjDic.Add(p.playerId, pObj);
        }
    }

    public GameObject GetPlayerObj(uint playerId)
    {
        if(playerObjDic.ContainsKey(playerId))
        {
            return playerObjDic[playerId];
        }
        else
        {
            return null;
        }
    }

    public void RemovePlayer(uint id)
    {
        if (playerInfoDic.ContainsKey(id))
        {
            playerInfoDic.Remove(id);

            Destroy(playerObjDic[id]);
            playerObjDic.Remove(id);
        }
    }
	
}
