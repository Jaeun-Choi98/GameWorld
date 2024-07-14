using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static PlayerData;

public class PlayerData : MonoBehaviour
{

  // 개발시 임의로 1로 설정
  public static int userId = 1;
  public int playerId = 0;
  public int jumpPower = 0;
  public int speed = 0;
  public int money = 0;
  public string playerName = null;

  // DB서버에 저장되지 않는 데이터
  public float hp = 20f;

  PlayerState playerState;
  PlatformPlayerState platformPlayerState;
  public class PlayerInfo
  {
    public int Speed;
    public int JumpPower;
    public int Money;
    public string Name;
  }

  public class PlayerDTO
  {
    public int userId;
    public int playerId;
    public PlayerInfo playerInfo;
  }

  private void Start()
  {
    playerState = GetComponent<PlayerState>();
    platformPlayerState = GetComponent<PlatformPlayerState>();
    StartCoroutine(LoadPlayerInfo());
    /*devModeInit();
    if(playerState != null)
    {
      playerState.PlayerStateLoad(hp, jumpPower, speed, money);
    }
    if(platformPlayerState != null)
    {
      platformPlayerState.PlatformPlayerStateLoad(speed, money, jumpPower, playerName);
    }*/
  }
  
  void devModeInit()
  {
    playerName = "cju";
    speed = 10;
    money = 1000;
    jumpPower = 5;
    playerId = 1;
  }

  IEnumerator LoadPlayerInfo()
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
      PlayerDTO playerDTO = JsonConvert.DeserializeObject<PlayerDTO>(jsonResponse);
      playerName = playerDTO.playerInfo.Name;
      speed = playerDTO.playerInfo.Speed;
      money = playerDTO.playerInfo.Money;
      jumpPower = playerDTO.playerInfo.JumpPower;
      playerId = playerDTO.playerId;
      hp = 20f;
      if (playerState != null)
      {
        playerState.PlayerStateLoad(hp, jumpPower, speed, money);
      }
      if (platformPlayerState != null)
      {
        platformPlayerState.PlatformPlayerStateLoad(speed, money, jumpPower, playerName);
      }
    }
  }

}
