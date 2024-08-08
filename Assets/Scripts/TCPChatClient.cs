using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Message
{
  public string type;
  public object payload;
}

// 클라이언트의 이름 설정 페이로드
public class SetNamePayload
{
  public string name;
}

// 채팅 메시지 페이로드
public class ChatMessagePayload
{
  public string message;
}

public class TCPChatClient : MonoBehaviour
{
  private TcpClient client;
  private NetworkStream stream;
  private StreamReader reader;
  private StreamWriter writer;
  public bool isChatOn;
  [SerializeField]
  private InputField chatInput;
  [SerializeField]
  private Text chatOutput;
  [SerializeField]
  private Transform chatUI;

  private string chatContent = "";
  private int maxMessages = 8;

  private void OnEnable()
  {
    isChatOn = false;
  }

  private void Start()
  {
    ConnectToServer("localhost", 5000);
    isChatOn = false;
    chatUI.gameObject.SetActive(false);
  }

  private void Update()
  {
    ReceiveMessage();
    if (Input.GetKeyDown(KeyCode.Return))
    {
      if (isChatOn)
      {
        Debug.Log(chatInput.text);
        SendMessage(chatInput.text);
        chatInput.text = "";
        chatUI.gameObject.SetActive(false);
      }
      else
      {
        // chat Input UI 열기
        chatUI.gameObject.SetActive(true);
      }
      isChatOn = !isChatOn;
    }
  }

  private void OnApplicationQuit()
  {
    Disconnect();
  }

  public void AddMessage(string message)
  {
    // 메시지를 추가하고, 줄바꿈 추가
    chatContent += message + "\n";

    // 메시지가 maxMessages를 초과할 경우, 가장 오래된 메시지를 제거
    int lineCount = chatContent.Split('\n').Length - 1;
    if (lineCount > maxMessages)
    {
      int firstLineIndex = chatContent.IndexOf('\n') + 1;
      chatContent = chatContent.Substring(firstLineIndex);
    }

    chatOutput.text = chatContent;

    Canvas.ForceUpdateCanvases();
  }

  void ConnectToServer(string addr, int port)
  {
    try
    {
      client = new TcpClient(addr, port);
      stream = client.GetStream();
      reader = new StreamReader(stream, Encoding.UTF8);
      writer = new StreamWriter(stream, Encoding.UTF8);

      Debug.Log("채팅 서버 연결");

      SendMessage(new Message
      {
        type = "SET_NAME",
        payload = new SetNamePayload { name = Server.Instance.playerInfo.Name }
      });
    }
    catch (Exception e)
    {
      Debug.LogError("채팅 서버 연결 에러: " + e.Message);
    }
  }

  void Disconnect()
  {
    if (writer != null) writer.Close();
    if (reader != null) reader.Close();
    if (client != null) client.Close();
    Debug.Log("서버와 연결 종료");
  }

  void SendMessage(Message msg)
  {
    if (stream != null && stream.CanWrite)
    {
      string json = JsonConvert.SerializeObject(msg);
      writer.WriteLine(json);
      writer.Flush();
      Debug.Log("보낸 메시지: " + json);
    }
  }

  new void SendMessage(string chatMsg)
  {
    SendMessage(new Message
    {
      type = "CHAT_MESSAGE",
      payload = new ChatMessagePayload { message = chatMsg }
    });
  }

  void ReceiveMessage()
  {
    if (stream != null && stream.DataAvailable)
    {
      string response = reader.ReadLine();
      if (!string.IsNullOrEmpty(response))
      {
        var resObj = JsonConvert.DeserializeObject<Message>(response);
        HandleMessage(resObj);
      }
    }
  }

  void HandleMessage(Message msg)
  {
    switch (msg.type)
    {
      case "CHAT_MESSAGE":
        Debug.Log("받은 메시지: " + msg.payload);
        var jsonPayload = JsonConvert.SerializeObject(msg.payload);
        var payload = JsonConvert.DeserializeObject<ChatMessagePayload>(jsonPayload);
        AddMessage(payload.message);
        break;
      default:
        Debug.Log("알려지지 않은 데이터 타입");
        break;
    }
  }
}
