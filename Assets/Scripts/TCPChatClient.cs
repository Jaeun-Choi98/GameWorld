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

// Ŭ���̾�Ʈ�� �̸� ���� ���̷ε�
public class SetNamePayload
{
  public string name;
}

// ä�� �޽��� ���̷ε�
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
        // chat Input UI ����
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
    // �޽����� �߰��ϰ�, �ٹٲ� �߰�
    chatContent += message + "\n";

    // �޽����� maxMessages�� �ʰ��� ���, ���� ������ �޽����� ����
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

      Debug.Log("ä�� ���� ����");

      SendMessage(new Message
      {
        type = "SET_NAME",
        payload = new SetNamePayload { name = Server.Instance.playerInfo.Name }
      });
    }
    catch (Exception e)
    {
      Debug.LogError("ä�� ���� ���� ����: " + e.Message);
    }
  }

  void Disconnect()
  {
    if (writer != null) writer.Close();
    if (reader != null) reader.Close();
    if (client != null) client.Close();
    Debug.Log("������ ���� ����");
  }

  void SendMessage(Message msg)
  {
    if (stream != null && stream.CanWrite)
    {
      string json = JsonConvert.SerializeObject(msg);
      writer.WriteLine(json);
      writer.Flush();
      Debug.Log("���� �޽���: " + json);
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
        Debug.Log("���� �޽���: " + msg.payload);
        var jsonPayload = JsonConvert.SerializeObject(msg.payload);
        var payload = JsonConvert.DeserializeObject<ChatMessagePayload>(jsonPayload);
        AddMessage(payload.message);
        break;
      default:
        Debug.Log("�˷����� ���� ������ Ÿ��");
        break;
    }
  }
}
