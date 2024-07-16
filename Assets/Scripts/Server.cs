using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class User
{
  public int userId;
  public string email;
  public string passwd;
}

[System.Serializable]
public class Player
{
  public int userId;
  public int playerId;
  public PlayerInfo playerInfo;
  public Inventory[] inventory;
}

[System.Serializable]
public class PlayerInfo
{
  public int Speed;
  public int JumpPower;
  public int Money;
  public string Name;
}

[System.Serializable]
public class Inventory
{
  public int ItemId;
  public int Quantity;
  public string ItemName;
}

public class Server : MonoBehaviour
{
  private static Server instance = null;

  public User user;
  
  public Player player;

  public PlayerInfo playerInfo;

  public Inventory[] inventory;

  public static Server Instance
  {
    get
    {
      if (instance == null)
      {
        return null;
      }
      return instance;
    }
  }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    // ���� ���� �� ����
    //InitDev();
  }

  private void InitDev()
  {
    player = new Player();
    playerInfo = new PlayerInfo();
    playerInfo.Speed = 10;
    playerInfo.JumpPower = 5;
    playerInfo.Name = "cjuTest";
    playerInfo.Money = 1111;
    player.userId = 1;
    player.playerId = 1;
    player.playerInfo = playerInfo;
  }

  public void SaveBossGameData(int userId, int playerId, string name, int money, int speed, int jumpPower)
  {
    StartCoroutine(SaveBossGameDataProcess(userId, playerId, name, money, speed, jumpPower));
    //StartCoroutine(LoadPlayerDataProcess(userId));
  }

  IEnumerator SaveBossGameDataProcess(int userId, int playerId, string name, int money, int speed, int jumpPower)
  {
    PlayerInfo info = new PlayerInfo();
    info.Name = name;
    info.JumpPower = jumpPower;
    info.Money = money;
    info.Speed = speed;
    Player player = new Player();
    player.userId = userId;
    player.playerId = playerId;
    player.playerInfo = info;

    string url = "localhost:8080/players/save-playerinfo";
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    string jsonStr = JsonConvert.SerializeObject(player);
    byte[] jsonStrToByte = new System.Text.UTF8Encoding().GetBytes(jsonStr);
    request.uploadHandler = new UploadHandlerRaw(jsonStrToByte);
    request.downloadHandler = new DownloadHandlerBuffer();

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      Player responseObj = JsonConvert.DeserializeObject<Player>(jsonResponse);
      Debug.Log("ȹ���� �� ��ȭ: " + responseObj.playerInfo.Money);
      yield return LoadPlayerDataProcess(userId);
    }
  }

  public void LoadPlayerData(int userId)
  {
    StartCoroutine(LoadPlayerDataProcess(userId));
  }

  IEnumerator LoadPlayerDataProcess(int userId)
  {
    string url = "localhost:8080/players/" + userId;
    UnityWebRequest request = new UnityWebRequest(url, "GET");
    request.downloadHandler = new DownloadHandlerBuffer();
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
      yield break;
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      //Debug.Log(jsonResponse);
      player = JsonConvert.DeserializeObject<Player>(jsonResponse);
      playerInfo = player.playerInfo;
      user.userId = userId;
      inventory = player.inventory;
    }
  }
}
