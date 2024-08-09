using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


[System.Serializable]
public class PlayerTransformPayload
{
  public int playerId;
  public float[] position;
  public float[] rotation;
  public bool isJump;
  public bool isRoll;
  public float h;
  public float v;
}

[System.Serializable]
public class DisconnectPayload
{
  public int playerId;
}

[System.Serializable]
public class ConnectPayload
{
  public int playerId;
}

public class TCPClient : MonoBehaviour
{
  [SerializeField]
  private PlayerManager playerManager;

  private TcpClient client;
  private NetworkStream stream;
  private StreamReader reader;
  private StreamWriter writer;

  [SerializeField]
  private float sendRate = 0.1f;
  private float nextSendTime;

  [SerializeField]
  private float consistanceSendRate = 2f;
  private float consistanceNextTime;

  private Vector3 lastSentPosition;
  [SerializeField]
  private float positionThreshold = 0.1f; // 위치 변화 임계값
  private Quaternion lastSentRotation;
  [SerializeField]
  private float rotationThreshold = 0.1f;

  [SerializeField]
  private PlayerMove playerMove;

  void Start()
  {
    ConnectToServer();
  }

  void Update()
  {
    // 서버로부터 메시지 수신
    if (stream != null && stream.DataAvailable)
    {
      string response = reader.ReadLine();
      if (!string.IsNullOrEmpty(response))
      {
        HandleMessage(response);
      }
    }

    if (Time.time >= nextSendTime)
    {
      nextSendTime = Time.time * sendRate;
      if (Vector3.Distance(transform.position, lastSentPosition) > positionThreshold || 
        Quaternion.Angle(transform.rotation, lastSentRotation) > rotationThreshold)
      {
        SendPlayerTransform();
        lastSentPosition = transform.position;
        lastSentRotation = transform.rotation;
      }
    }

    if (Time.time >= consistanceNextTime)
    {
      consistanceNextTime = Time.time * consistanceSendRate;
      SendPlayerTransform();
      lastSentPosition = transform.position;
      lastSentRotation = transform.rotation;
    }
  }

  void OnApplicationQuit()
  {
    Disconnect();
  }

  void ConnectToServer()
  {
    try
    {
      client = new TcpClient("localhost", 5001);
      stream = client.GetStream();
      reader = new StreamReader(stream, Encoding.UTF8);
      writer = new StreamWriter(stream, Encoding.UTF8);

      Debug.Log("Connected to server");

      // 클라이언트 이름 설정 메시지 전송
      SendMessageToServer(new Message
      {
        type = "CONNECT",
        payload = new { playerId = Server.Instance.player.playerId }
      });
    }
    catch (Exception e)
    {
      Debug.LogError("Error connecting to server: " + e.Message);
    }
  }

  void Disconnect()
  {
    if (writer != null) writer.Close();
    if (reader != null) reader.Close();
    if (client != null) client.Close();
    Debug.Log("Disconnected from server");
  }

  public void SendMessageToServer(Message msg)
  {
    if (stream != null && stream.CanWrite)
    {
      string json = JsonConvert.SerializeObject(msg);
      writer.WriteLine(json);
      writer.Flush();
      Debug.Log("Message sent to server: " + json);
    }
  }

  void HandleMessage(string response)
  {
    // 서버로부터 받은 메시지 처리
    Debug.Log("Message received from server: " + response);

    // 서버로부터 받은 메시지를 처리하여 다른 플레이어의 위치 업데이트
    Message msg = JsonConvert.DeserializeObject<Message>(response);
    switch (msg.type)
    {
      case "CONNECT":
        ConnectPayload connectPayload = JsonUtility.FromJson<ConnectPayload>(msg.payload.ToString());
        playerManager.SpawnPlayer(connectPayload.playerId, new Vector3(0,1,0));
        break;
      case "PLAYER_TRANSFORM":
        PlayerTransformPayload transformPayload = JsonUtility.FromJson<PlayerTransformPayload>(msg.payload.ToString());
        playerManager.UpdatePlayerTransform(transformPayload);
        break;
      case "DISCONNECT":
        DisconnectPayload disconnectPayload = JsonUtility.FromJson<DisconnectPayload>(msg.payload.ToString());
        playerManager.RemovePlayer(disconnectPayload.playerId);
        break;
    }
  }

  public void SendPlayerTransform()
  {
    Vector3 position = transform.position;
    Quaternion rotation = transform.rotation;

    PlayerTransformPayload payload = new PlayerTransformPayload
    {
      playerId = Server.Instance.player.playerId,
      position = new float[] { position.x, position.y, position.z },
      rotation = new float[] { rotation.x, rotation.y, rotation.z, rotation.w },
      isJump = playerMove.isJump,
      isRoll = playerMove.isRoll,
      h = playerMove.h,
      v = playerMove.v,
    };

    SendMessageToServer(new Message
    {
      type = "PLAYER_TRANSFORM",
      payload = payload
    });
  }

}
