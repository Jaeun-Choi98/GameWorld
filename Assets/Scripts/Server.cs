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
}

[System.Serializable]
public class PlayerInfo
{
  public int Speed;
  public int JumpPower;
  public int Money;
  public string Name;
}

public class Server : MonoBehaviour
{
  private static Server instance = null;

  public User user;
  
  public Player player;

  public PlayerInfo playerInfo;

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

    string url = "localhost:8080/players/save";
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
      Debug.Log("»πµÊ«— √— ¿Á»≠: " + responseObj.playerInfo.Money);
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
    }
  }
}
